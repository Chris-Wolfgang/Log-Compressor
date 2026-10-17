type: internal

Elapsed time is taken with `Stopwatch.GetElapsedTime` instead of start/stop, and two guards that other code already made redundant (the response-file argument length check, the null check in date parsing) are removed; no behaviour change.
