using McMaster.Extensions.CommandLineUtils;
using Serilog;
using TypeForge.Cli;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

using var cli = new CommandLineApplication<TypeForgeOptions>();
cli.Conventions.UseDefaultConventions();
return cli.Execute(args);
