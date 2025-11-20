using Microsoft.EntityFrameworkCore;
using PipocaResenha.Data;
using PipocaResenha.Services;

var builder = WebApplication.CreateBuilder(args);

// registra MemoryCache
builder.Services.AddMemoryCache();

builder.Services.AddHttpClient<ITmdbService, TmdbService>(client =>
{
    client.BaseAddress = new Uri("https://api.themoviedb.org/3/");
});

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("StringConexao")));

builder.Services.AddAuthentication("CookieAuth").AddCookie("CookieAuth", options => {
    options.LoginPath = "/Account/Login";
});

var app = builder.Build();

if (!app.Environment.IsDevelopment()) app.UseExceptionHandler("/Home/Error");
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{codigo?}");

// Habilita endpoints com rotas por atributo (API controllers)
app.MapControllers();

app.Run();