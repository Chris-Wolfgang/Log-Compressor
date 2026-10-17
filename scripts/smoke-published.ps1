#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Runs a published logc binary through the behaviours a trimmed build could silently break.

.DESCRIPTION
    Trimming removes code the linker cannot see being used. McMaster.Extensions.CommandLineUtils
    and System.Text.Json both find members by name or by reflection, so a binary can build
    cleanly and still fail at run time (the first trimmed build failed every command, and then
    `--report json`). Unit tests run the un-trimmed assemblies and cannot see that, so this
    script drives the published executable itself.

    It checks, against the real binary: version and help for every command; a compress -> decompress
    round trip for every format with SHA-256 verification of the restored content; bundling; the
    JSON report; the exit code of each kind of invalid input; and an edited `logc init` response file
    that still has its blank lines (the shape that used to fail with "Unrecognized command or argument").

    Exits 0 when every check passes and 1 otherwise, printing each failure with the command output.
    Needs PowerShell 7+; runs the same on Windows, Linux and macOS.

.PARAMETER Exe
    Path to the published logc executable (logc.exe on Windows, logc elsewhere).

.PARAMETER WorkDirectory
    Scratch directory for the generated fixtures. Defaults to a new directory under the system
    temp path, removed on exit.

.EXAMPLE
    pwsh ./scripts/smoke-published.ps1 -Exe ./publish/linux-x64/logc
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string]$Exe,
    [string]$WorkDirectory
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

if (-not (Test-Path -LiteralPath $Exe -PathType Leaf)) { throw "Executable not found: $Exe" }
$Exe = (Resolve-Path -LiteralPath $Exe).Path

$ownsWorkDirectory = -not $WorkDirectory
if ($ownsWorkDirectory) { $WorkDirectory = Join-Path ([IO.Path]::GetTempPath()) ("logc-smoke-" + [guid]::NewGuid().ToString('N')) }
New-Item -ItemType Directory -Force -Path $WorkDirectory | Out-Null

$script:failures = [System.Collections.Generic.List[string]]::new()
$script:checks = 0

function Invoke-Logc {
    param([string[]]$Arguments)
    $text = (& $Exe @Arguments 2>&1 | Out-String)
    [pscustomobject]@{ Exit = $LASTEXITCODE; Output = $text }
}

function Assert-Check {
    param([string]$Name, [bool]$Condition, [string]$Detail = '')
    $script:checks++
    if ($Condition) {
        Write-Host "  ok    $Name"
    }
    else {
        Write-Host "  FAIL  $Name"
        $script:failures.Add("$Name`n$Detail")
    }
}

function New-Fixture {
    # Three small, distinct files (one in a sub-directory), enough text for every format to compress.
    $dir = Join-Path $WorkDirectory ("fx-" + [guid]::NewGuid().ToString('N'))
    New-Item -ItemType Directory -Force -Path (Join-Path $dir 'sub') | Out-Null
    foreach ($name in 'a', 'b') {
        $lines = 1..400 | ForEach-Object { "line $_ of ${name}: the quick brown fox jumps over the lazy dog 0123456789" }
        Set-Content -LiteralPath (Join-Path $dir "$name.log") -Value $lines -Encoding utf8
    }
    Set-Content -LiteralPath (Join-Path $dir 'sub/c.log') -Value (1..200 | ForEach-Object { "nested $_" }) -Encoding utf8
    $dir
}

function Get-ContentHashes {
    param([string]$Directory)
    @(Get-ChildItem -LiteralPath $Directory -Recurse -File | ForEach-Object { (Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash })
}

try {
    Write-Host "logc smoke test: $Exe ($([math]::Round((Get-Item -LiteralPath $Exe).Length / 1MB, 1)) MB)"

    Write-Host 'version and help'
    $r = Invoke-Logc @('--version')
    Assert-Check '--version exits 0 and prints a version' ($r.Exit -eq 0 -and $r.Output.Trim() -match '^\d+\.\d+\.\d+') $r.Output
    foreach ($command in @(@(), @('compress'), @('bundle'), @('decompress'), @('init'))) {
        $r = Invoke-Logc ($command + '--help')
        $label = ("$($command -join ' ') --help").Trim()
        Assert-Check "$label exits 0 and shows usage" ($r.Exit -eq 0 -and $r.Output -match 'Usage: logc') $r.Output
    }

    Write-Host 'compress -> decompress round trip, per format'
    foreach ($format in 'zip', 'gz', 'brotli', 'zstd', 'lz4') {
        $dir = New-Fixture
        $original = Get-ContentHashes $dir
        $r = Invoke-Logc @('compress', $dir, '-f', $format, '-r')
        Assert-Check "compress -f $format exits 0" ($r.Exit -eq 0 -and $r.Output -match 'Compressed 3 file') $r.Output
        $r = Invoke-Logc @('decompress', $dir, '-r')
        Assert-Check "decompress ($format) exits 0" ($r.Exit -eq 0) $r.Output
        $restored = Get-ContentHashes $dir
        $missing = @($original | Where-Object { $restored -notcontains $_ })
        Assert-Check "$format round trip restores every file byte for byte" ($missing.Count -eq 0) "missing hashes: $($missing -join ', ')"
    }

    Write-Host 'bundle'
    foreach ($format in 'zip', 'gz') {
        $dir = New-Fixture
        $r = Invoke-Logc @('bundle', $dir, '-f', $format, '-r')
        Assert-Check "bundle -f $format exits 0" ($r.Exit -eq 0 -and $r.Output -match 'Bundled files') $r.Output
    }

    Write-Host 'JSON report'
    $dir = New-Fixture
    $reportPath = Join-Path $dir 'report.json'
    $r = Invoke-Logc @('compress', $dir, '-f', 'gz', '-r', '--report', 'json', '--report-path', $reportPath)
    Assert-Check 'compress --report json exits 0' ($r.Exit -eq 0) $r.Output
    if (Test-Path -LiteralPath $reportPath) {
        $report = Get-Content -LiteralPath $reportPath -Raw | ConvertFrom-Json
        $keys = @($report.PSObject.Properties.Name)
        $expectedKeys = 'timestamp', 'duration', 'totalFiles', 'succeededFiles', 'failedFiles', 'originalSizeBytes', 'compressedSizeBytes', 'files', 'errors'
        Assert-Check 'report has every expected key, in order' (($keys -join ',') -eq ($expectedKeys -join ',')) "keys: $($keys -join ',')"
        Assert-Check 'report counts three succeeded files' ($report.totalFiles -eq 3 -and $report.succeededFiles -eq 3 -and $report.failedFiles -eq 0) ($report | ConvertTo-Json -Depth 3)
        Assert-Check 'report lists three files and no errors' (@($report.files).Count -eq 3 -and @($report.errors).Count -eq 0) ($report | ConvertTo-Json -Depth 3)
    }
    else {
        Assert-Check 'report file was written' $false "no file at $reportPath"
    }

    Write-Host 'invalid input exits with the documented code'
    $dir = New-Fixture
    $cases = @(
        @{ Name = '--older-than 0'; Arguments = @('compress', $dir, '--older-than', '0'); Exit = 1 },
        @{ Name = 'missing path'; Arguments = @('compress'); Exit = 1 },
        @{ Name = 'unsupported format'; Arguments = @('compress', $dir, '-f', 'nonsense'); Exit = 1 },
        @{ Name = 'unknown option'; Arguments = @('compress', $dir, '--bogus'); Exit = 10 }
    )
    foreach ($case in $cases) {
        $r = Invoke-Logc $case.Arguments
        Assert-Check "$($case.Name) exits $($case.Exit)" ($r.Exit -eq $case.Exit) "exit $($r.Exit): $($r.Output)"
    }

    Write-Host 'init response file, edited as its header says, blank lines kept'
    $dir = New-Fixture
    $rsp = Join-Path $WorkDirectory ("compress-" + [guid]::NewGuid().ToString('N') + '.rsp')
    $r = Invoke-Logc @('init', 'compress', '-o', $rsp)
    Assert-Check 'init compress writes the template' ($r.Exit -eq 0 -and (Test-Path -LiteralPath $rsp)) $r.Output
    if (Test-Path -LiteralPath $rsp) {
        $lines = @(Get-Content -LiteralPath $rsp)
        $blank = @($lines | Where-Object { $_.Trim().Length -eq 0 }).Count
        Assert-Check 'the template contains blank lines (what used to break parsing)' ($blank -gt 0) "blank lines: $blank"
        $edited = $lines | ForEach-Object {
            if ($_ -match '^# [A-Za-z]:\\Logs\\MyApp$') { $dir }
            elseif ($_ -eq '# --recurse') { '--recurse' }
            else { $_ }
        }
        Set-Content -LiteralPath $rsp -Value $edited -Encoding utf8
        $r = Invoke-Logc @('compress', "@$rsp")
        Assert-Check 'compress @file works with the blank lines still in it' ($r.Exit -eq 0 -and $r.Output -match 'Compressed 3 file') $r.Output
    }
}
finally {
    if ($ownsWorkDirectory -and (Test-Path -LiteralPath $WorkDirectory)) { Remove-Item -LiteralPath $WorkDirectory -Recurse -Force -ErrorAction SilentlyContinue }
}

Write-Host ''
if ($script:failures.Count -eq 0) {
    Write-Host "All $($script:checks) checks passed."
    exit 0
}

Write-Host "$($script:failures.Count) of $($script:checks) checks FAILED:"
foreach ($failure in $script:failures) { Write-Host "::error::$($failure -replace "`n", ' | ')" }
exit 1
