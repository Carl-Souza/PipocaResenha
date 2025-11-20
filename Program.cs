using PipocaResenha.Services;

var builder = WebApplication.CreateBuilder(args);

// ... outras configurações

// registra MemoryCache
builder.Services.AddMemoryCache();

builder.Services.AddHttpClient<ITmdbService, TmdbService>(client =>
{
    client.BaseAddress = new Uri("https://api.themoviedb.org/3/");
});

// ... restante