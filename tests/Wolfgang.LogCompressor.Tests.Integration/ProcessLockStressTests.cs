using Microsoft.Extensions.Logging.Abstractions;
using Wolfgang.LogCompressor.Service;

namespace Wolfgang.LogCompressor.Tests.Integration;

/// <summary>
/// Concurrency stress tests for <see cref="ProcessLock"/> (fleet issue #76):
/// many contenders race the acquire path simultaneously and the mutual-
/// exclusion invariant must hold on every schedule the OS produces. Iteration
/// count scales via the STRESS_ITERATIONS environment variable (modest per-PR
/// default, crank it up for soak runs). Per the fleet-wide decision the
/// systematic-exploration tool (Coyote) is skipped — the lock's concurrency
/// is OS file-system arbitration, which Coyote cannot schedule anyway; racing
/// real handles is the honest test.
/// </summary>
public sealed class ProcessLockStressTests
{
    private const int Contenders = 16;



    private static int Iterations =>
        int.TryParse(Environment.GetEnvironmentVariable("STRESS_ITERATIONS"), out var n) && n > 0
            ? n
            : 25;



    [Fact]
    public async Task TryAcquire_when_manyConcurrentContenders_expected_exactlyOneWinner()
    {
        for (var i = 0; i < Iterations; i++)
        {
            using var temp = new TempDirectory();
            var locks = Enumerable
                .Range(0, Contenders)
                .Select(_ => new ProcessLock(temp.Path, NullLogger.Instance))
                .ToList();

            try
            {
                using var barrier = new Barrier(Contenders);

                // Task.WhenAll completes before the locks are disposed, so the
                // closures never observe a disposed lock.
                // ReSharper disable once AccessToDisposedClosure
                var results = await Task.WhenAll
                (
                    locks.Select(l => Task.Run(() =>
                    {
                        // All contenders hit TryAcquire as simultaneously as
                        // the scheduler allows.
                        barrier.SignalAndWait();
                        return l.TryAcquire();
                    }))
                );

                Assert.Equal(1, results.Count(r => r));
            }
            finally
            {
                foreach (var l in locks)
                {
                    l.Dispose();
                }
            }
        }
    }



    [Fact]
    public async Task TryAcquire_when_leftoverFileContended_expected_exactlyOneWinner()
    {
        // The #172 regression case: a leftover (unheld) lock file exists and
        // the whole field races the takeover. Under the OpenOrCreate protocol
        // every contender opens the same file and exactly one wins the
        // exclusive share — the old check/delete/create sequence could
        // produce two winners here on Linux.
        for (var i = 0; i < Iterations; i++)
        {
            using var temp = new TempDirectory();
            temp.WriteFile(ProcessLock.LockFileName, "PID=0\nStarted=2020-01-01T00:00:00Z\n");

            var locks = Enumerable
                .Range(0, Contenders)
                .Select(_ => new ProcessLock(temp.Path, NullLogger.Instance))
                .ToList();

            try
            {
                using var barrier = new Barrier(Contenders);

                // Task.WhenAll completes before the locks are disposed, so the
                // closures never observe a disposed lock.
                // ReSharper disable once AccessToDisposedClosure
                var results = await Task.WhenAll
                (
                    locks.Select(l => Task.Run(() =>
                    {
                        barrier.SignalAndWait();
                        return l.TryAcquire();
                    }))
                );

                Assert.Equal(1, results.Count(r => r));
            }
            finally
            {
                foreach (var l in locks)
                {
                    l.Dispose();
                }
            }
        }
    }



    [Fact]
    public async Task TryAcquire_when_contendedHandoffCycles_expected_everyCycleHasOneHolder()
    {
        // Acquire/release churn: each cycle the previous winner releases and
        // the whole field races again. Total successes must equal the number
        // of cycles — no cycle with zero winners (lost lock file) and none
        // with two (broken exclusion).
        using var temp = new TempDirectory();
        var totalWins = 0;

        for (var cycle = 0; cycle < Iterations; cycle++)
        {
            var locks = Enumerable
                .Range(0, Contenders)
                .Select(_ => new ProcessLock(temp.Path, NullLogger.Instance))
                .ToList();

            try
            {
                using var barrier = new Barrier(Contenders);

                // Task.WhenAll completes before the locks are disposed, so the
                // closures never observe a disposed lock.
                // ReSharper disable once AccessToDisposedClosure
                var results = await Task.WhenAll
                (
                    locks.Select(l => Task.Run(() =>
                    {
                        barrier.SignalAndWait();
                        return l.TryAcquire();
                    }))
                );

                var wins = results.Count(r => r);
                Assert.Equal(1, wins);
                totalWins += wins;
            }
            finally
            {
                // Disposing the winner deletes the lock file, handing the
                // next cycle a clean directory.
                foreach (var l in locks)
                {
                    l.Dispose();
                }
            }
        }

        Assert.Equal(Iterations, totalWins);
    }
}
