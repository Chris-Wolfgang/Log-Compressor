type: feature

Release binaries are now Native AOT executables for win-x64, linux-x64 and osx-arm64: no .NET runtime, faster startup, and a smaller download (measured on Windows: `logc.exe` 11.7 MB, from 13.6 MB trimmed single-file). Behaviour is unchanged, checked against the JIT build by `scripts/smoke-published.ps1`.
