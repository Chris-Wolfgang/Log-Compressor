type: fix

logc's own log file (`Logging:File:Path`, default `logs/log-.txt`) is now written next to the executable when the path is relative, instead of under whatever working directory the scheduler started it in; two unused packages (Serilog.Exceptions, System.Configuration.ConfigurationManager) are removed, which drops their trim warnings and shrinks the self-contained binary.
