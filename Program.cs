using Microsoft.EntityFrameworkCore;
using oblig.DAL;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register DbContext with SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("AppDbContextConnection"));
});

// Register repositories
builder.Services.AddScoped<IQuizRepository, QuizRepository>();


// Configure and add Serilog
var loggerConfiguration = new LoggerConfiguration().MinimumLevel.Information().WriteTo.File($"Logs/app_{DateTime.Now:yyyyMMddHHmmss}.log");
loggerConfiguration.Filter.ByExcluding(e => e.Properties.TryGetValue("SourceContext", out var value) &&
    e.Level == LogEventLevel.Information &&
    e.MessageTemplate.Text.Contains("Execute DbCommand")
);
var logger = loggerConfiguration.CreateLogger();
builder.Logging.AddSerilog(logger);

var app = builder.Build();

// Seed database in development
if (app.Environment.IsDevelopment())
{
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
