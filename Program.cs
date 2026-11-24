
using SarabRestaurant.Services;
using Serilog;
using Microsoft.AspNetCore.Http.Extensions;

var builder = WebApplication.CreateBuilder(args);

var logsPath = Path.Combine(
    builder.Environment.ContentRootPath, "Logs");

Directory.CreateDirectory(logsPath);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
    .WriteTo.Console()
    .WriteTo.File(
        Path.Combine(logsPath, "requests-.log"),
        rollingInterval: RollingInterval.Day,
        outputTemplate:
            "{Timestamp:yyyy-MM-dd HH:mm:ss} | {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllersWithViews();

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddTransient<IEmailSender>(provider =>
{
    var settings = provider
        .GetRequiredService<
            Microsoft.Extensions.Options.IOptions<EmailSettings>>()
        .Value;

    return new EmailSender(settings);
});

var app = builder.Build();

app.Use(async (context, next) =>
{
    var url = context.Request.GetDisplayUrl();

    var ip = context.Connection.RemoteIpAddress?.ToString()
        ?? "Unknown";

    Log.Information(
        "URL: {Url} | IP: {IP}",
        url, ip);

    await next();
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
