using Serilog;
using TypeForge.AspNetCore.Configuration;
using TypeForge.Core.Configuration;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
var logger = Log.Logger.ForContext<Program>();

// builder.WebHost.UseWebRoot("wwwroot");
// builder.WebHost.UseStaticWebAssets();
// Add services to the container.
// builder.Environment.WebRootPath;
logger.Information("builder.Environment.WebRootPath {A}", builder.Environment.WebRootPath);

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/*
builder.Services.AddTypeForge(options =>
{
    options.FileNameCase = FileNameCase.CamelCase;
    options.
});*/

builder.Services.AddTypeForgeConfigFile();

builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseFileServer();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// app.UseStaticFiles();
app.UseRouting();

// app.MapRazorPages();
app.UseTypeForge();

app.Run();
