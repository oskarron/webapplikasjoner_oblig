using Microsoft.EntityFrameworkCore;
using oblig.DAL;

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

var app = builder.Build();

// Seed database in development
if (app.Environment.IsDevelopment())
{
    DbInit.Seed(app);
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
