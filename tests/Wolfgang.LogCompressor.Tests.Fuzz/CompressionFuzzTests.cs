using System.Formats.Tar;
using System.IO.Compression;
using CsCheck;
using Microsoft.Extensions.Logging.Abstractions;
using Wolfgang.LogCompressor.Abstraction;
using Wolfgang.LogCompressor.Service;
using Wolfgang.LogCompressor.Service.Compression;
using Xunit;

namespace Wolfgang.LogCompressor.Tests.Fuzz;

/// <summary>
/// Property-based fuzz tests (#68) over the compression strategies, the
/// archive verifier and the file filter, using CsCheck. The load-bearing
/// invariant is the round-trip: whatever bytes go into an archive must come
/// back out unchanged through an independent BCL reader — this is the tool
/// that DELETES originals after compressing, so a silent corruption is the
/// worst bug it can have.
/// The case count is <c>FUZZ_ITER</c> (default 500 for the per-PR run); the
/// scheduled fuzz.yaml sets it far higher for a deep sweep. On failure
/// CsCheck shrinks to a minimal input and prints a replayable seed.
/// </summary>
public class CompressionFuzzTests
{
    private static long Iterations =>
        long.TryParse(Environment.GetEnvironmentVariable("FUZZ_ITER"), out var n) && n > 0
            ? n
            : 500;



    // Arbitrary binary content, biased small for throughput but reaching a few
    // buffer-boundary-crossing sizes. Empty is a valid log file.
    private static readonly Gen<byte[]> Content =
        Gen.Byte.Array[0, 8192];

    // Entry names as they come from real file names: safe cross-platform
    // characters, including dots (my.app.log-style names).
    private static readonly Gen<string> EntryName =
        Gen.Select
        (
            Gen.String[Gen.Char.AlphaNumeric, 1, 20],
            Gen.OneOfConst(".log", ".txt", ""),
            (stem, ext) => stem + ext
        );



    private static byte[] CompressSingle(ICompressionStrategy strategy, byte[] content, string entryName)
    {
        using var input = new MemoryStream(content);
        using var output = new MemoryStream();
        // Safe sync-over-async: MemoryStream operations complete synchronously,
        // and CsCheck Sample lambdas must be synchronous.
#pragma warning disable VSTHRD002
        strategy.CompressFileAsync(input, output, entryName).GetAwaiter().GetResult();
#pragma warning restore VSTHRD002
        return output.ToArray();
    }



    private static byte[] CompressBundle(ICompressionStrategy strategy, IReadOnlyList<(byte[] Content, string Name)> entries)
    {
        using var output = new MemoryStream();
        // Safe sync-over-async: MemoryStream operations complete synchronously,
        // and CsCheck Sample lambdas must be synchronous.
#pragma warning disable VSTHRD002
        strategy
            .CompressFilesAsync
            (
                entries.Select(e => ((Stream)new MemoryStream(e.Content), e.Name)),
                output
            )
            .GetAwaiter()
            .GetResult();
#pragma warning restore VSTHRD002
        return output.ToArray();
    }



    [Fact]
    public void Zip_single_file_round_trips_through_independent_reader()
    {
        Gen.Select(Content, EntryName).Sample(
            (content, entryName) =>
            {
                var archive = CompressSingle(new ZipCompressionStrategy(), content, entryName);

                using var zip = new ZipArchive(new MemoryStream(archive), ZipArchiveMode.Read);
                var entry = Assert.Single(zip.Entries);
                Assert.Equal(entryName, entry.Name);

                using var extracted = new MemoryStream();
                using (var entryStream = entry.Open())
                {
                    entryStream.CopyTo(extracted);
                }

                Assert.Equal(content, extracted.ToArray());
            },
            iter: Iterations);
    }



    [Fact]
    public void GZip_single_file_round_trips_through_independent_reader()
    {
        Content.Sample(
            content =>
            {
                var archive = CompressSingle(new GZipCompressionStrategy(), content, "any.log");

                using var extracted = new MemoryStream();
                using (var gz = new GZipStream(new MemoryStream(archive), CompressionMode.Decompress))
                {
                    gz.CopyTo(extracted);
                }

                Assert.Equal(content, extracted.ToArray());
            },
            iter: Iterations);
    }



    [Fact]
    public void Brotli_single_file_round_trips_through_independent_reader()
    {
        Content.Sample(
            content =>
            {
                var archive = CompressSingle(new BrotliCompressionStrategy(), content, "any.log");

                using var extracted = new MemoryStream();
                using (var br = new BrotliStream(new MemoryStream(archive), CompressionMode.Decompress))
                {
                    br.CopyTo(extracted);
                }

                Assert.Equal(content, extracted.ToArray());
            },
            iter: Iterations);
    }



    // Bundle entries: 1-5 files, names made unique with an index suffix so the
    // round-trip can match entries back unambiguously.
    private static readonly Gen<List<(byte[] Content, string Name)>> BundleEntries =
        Gen.Select(Content, EntryName)
            .List[1, 5]
            .Select(list => list
                .Select((e, i) => (e.Item1, $"{i}-{e.Item2}"))
                .ToList());



    [Fact]
    public void Zip_bundle_round_trips_every_entry()
    {
        BundleEntries.Sample(
            entries =>
            {
                var archive = CompressBundle(new ZipCompressionStrategy(), entries);

                using var zip = new ZipArchive(new MemoryStream(archive), ZipArchiveMode.Read);
                Assert.Equal(entries.Count, zip.Entries.Count);

                foreach (var (content, name) in entries)
                {
                    var entry = zip.GetEntry(name);
                    Assert.NotNull(entry);

                    using var extracted = new MemoryStream();
                    using (var entryStream = entry.Open())
                    {
                        entryStream.CopyTo(extracted);
                    }

                    Assert.Equal(content, extracted.ToArray());
                }
            },
            iter: Iterations);
    }



    [Fact]
    public void GZip_bundle_round_trips_every_entry_through_tar_reader()
    {
        BundleEntries.Sample(
            entries => AssertTarBundleRoundTrips
            (
                entries,
                CompressBundle(new GZipCompressionStrategy(), entries),
                s => new GZipStream(s, CompressionMode.Decompress)
            ),
            iter: Iterations);
    }



    [Fact]
    public void Brotli_bundle_round_trips_every_entry_through_tar_reader()
    {
        BundleEntries.Sample(
            entries => AssertTarBundleRoundTrips
            (
                entries,
                CompressBundle(new BrotliCompressionStrategy(), entries),
                s => new BrotliStream(s, CompressionMode.Decompress)
            ),
            iter: Iterations);
    }



    private static void AssertTarBundleRoundTrips
    (
        IReadOnlyList<(byte[] Content, string Name)> entries,
        byte[] archive,
        Func<Stream, Stream> decompressor
    )
    {
        var extracted = new Dictionary<string, byte[]>(StringComparer.Ordinal);

        using (var decompressed = decompressor(new MemoryStream(archive)))
        using (var tar = new TarReader(decompressed))
        {
            while (tar.GetNextEntry() is { } entry)
            {
                using var buffer = new MemoryStream();
                entry.DataStream?.CopyTo(buffer);
                extracted[entry.Name] = buffer.ToArray();
            }
        }

        Assert.Equal(entries.Count, extracted.Count);

        foreach (var (content, name) in entries)
        {
            Assert.True(extracted.TryGetValue(name, out var bytes), $"missing entry {name}");
            Assert.Equal(content, bytes);
        }
    }



    [Fact]
    public void Zstd_single_file_round_trips_through_decompression()
    {
        Content.Sample(
            content =>
            {
                var archive = CompressSingle(new ZstdCompressionStrategy(), content, "any.log");

                using var extracted = new MemoryStream();
                using (var zstd = new ZstdSharp.DecompressionStream(new MemoryStream(archive)))
                {
                    zstd.CopyTo(extracted);
                }

                Assert.Equal(content, extracted.ToArray());
            },
            iter: Iterations);
    }



    [Fact]
    public void Lz4_single_file_round_trips_through_decompression()
    {
        Content.Sample(
            content =>
            {
                var archive = CompressSingle(new Lz4CompressionStrategy(), content, "any.log");

                using var extracted = new MemoryStream();
                using (var lz4 = K4os.Compression.LZ4.Streams.LZ4Stream.Decode(new MemoryStream(archive)))
                {
                    lz4.CopyTo(extracted);
                }

                Assert.Equal(content, extracted.ToArray());
            },
            iter: Iterations);
    }



    [Fact]
    public void Zstd_bundle_round_trips_every_entry_through_tar_reader()
    {
        BundleEntries.Sample(
            entries => AssertTarBundleRoundTrips
            (
                entries,
                CompressBundle(new ZstdCompressionStrategy(), entries),
                s => new ZstdSharp.DecompressionStream(s)
            ),
            iter: Iterations);
    }



    [Fact]
    public void Lz4_bundle_round_trips_every_entry_through_tar_reader()
    {
        BundleEntries.Sample(
            entries => AssertTarBundleRoundTrips
            (
                entries,
                CompressBundle(new Lz4CompressionStrategy(), entries),
                s => K4os.Compression.LZ4.Streams.LZ4Stream.Decode(s)
            ),
            iter: Iterations);
    }



    [Fact]
    public void Verifier_accepts_every_archive_the_strategies_produce()
    {
        Gen.Select(Content, Gen.OneOfConst("zip", "gz", "br", "zst", "lz4")).Sample(
            (content, format) =>
            {
                ICompressionStrategy strategy = format switch
                {
                    "zip" => new ZipCompressionStrategy(),
                    "gz" => new GZipCompressionStrategy(),
                    "zst" => new ZstdCompressionStrategy(),
                    "lz4" => new Lz4CompressionStrategy(),
                    _ => new BrotliCompressionStrategy()
                };

                using var temp = new TempDirectory();
                var path = Path.Combine(temp.Path, "archive." + format);
                File.WriteAllBytes(path, CompressSingle(strategy, content, "file.log"));

                var verifier = new ArchiveVerifier(NullLogger<ArchiveVerifier>.Instance);
                // Safe sync-over-async: CsCheck Sample lambdas must be synchronous.
#pragma warning disable VSTHRD002
                Assert.True(verifier.VerifyAsync(path, format, content.Length).GetAwaiter().GetResult());
#pragma warning restore VSTHRD002
            },
            iter: Iterations);
    }



    [Fact]
    public void Verifier_never_passes_a_truncated_archive_whose_content_is_lost()
    {
        // The safety contract behind verify-then-delete (ADR-0003): a passing
        // verification means every source byte is recoverable — because a
        // pass is what authorises deleting the original. Truncation must
        // therefore either fail verification OR provably not have lost
        // anything (brotli's end-of-stream framing can be cut without losing
        // data; the format has no checksum, so that case legitimately
        // verifies). Motivated by the fuzz finding that modern .NET's
        // GZipStream/BrotliStream return partial data on truncated input
        // instead of throwing, so "decompress without error" alone proves
        // nothing. Zip fails via its central directory, gzip via its
        // CRC/length trailer, brotli via the expected-size comparison.
        Gen.Select(Gen.Byte.Array[1, 4096], Gen.OneOfConst("zip", "gz", "br"), Gen.Int[1, 64]).Sample(
            (content, format, cut) =>
            {
                ICompressionStrategy strategy = format switch
                {
                    "zip" => new ZipCompressionStrategy(),
                    "gz" => new GZipCompressionStrategy(),
                    _ => new BrotliCompressionStrategy()
                };

                var archive = CompressSingle(strategy, content, "file.log");
                var truncated = archive[..Math.Max(1, archive.Length - cut)];

                using var temp = new TempDirectory();
                var path = Path.Combine(temp.Path, "archive." + format);
                File.WriteAllBytes(path, truncated);

                var verifier = new ArchiveVerifier(NullLogger<ArchiveVerifier>.Instance);
                // Safe sync-over-async: CsCheck Sample lambdas must be synchronous.
#pragma warning disable VSTHRD002
                var verified = verifier.VerifyAsync(path, format, content.Length).GetAwaiter().GetResult();
#pragma warning restore VSTHRD002

                if (verified)
                {
                    // Zip and gzip carry end-of-stream integrity data, so a
                    // truncated archive must NEVER pass — only brotli has the
                    // legitimate cut-framing-only case.
                    Assert.Equal("br", format);

                    // And even then, a pass is only acceptable when the
                    // content survived the truncation byte-for-byte.
                    Assert.Equal(content, DecodeBrotli(truncated));
                }
            },
            iter: Iterations);
    }



    private static byte[] DecodeBrotli(byte[] archive)
    {
        using var extracted = new MemoryStream();
        using (var br = new BrotliStream(new MemoryStream(archive), CompressionMode.Decompress))
        {
            br.CopyTo(extracted);
        }

        return extracted.ToArray();
    }
}
