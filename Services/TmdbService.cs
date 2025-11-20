using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace PipocaResenha.Services
{
    public class TmdbService : ITmdbService
    {
        private readonly HttpClient _http;
        private readonly string _apiKey;
        private readonly string _imageBase = "https://image.tmdb.org/t/p/w500";
        private readonly IMemoryCache _cache;

        public TmdbService(HttpClient http, IConfiguration config, IMemoryCache cache)
        {
            _http = http;
            _apiKey = config["Tmdb:ApiKey"] ?? throw new ArgumentNullException("Tmdb:ApiKey não configurado");
            _cache = cache;
        }

        public async Task<IEnumerable<TmdbMovieDto>> GetHomeMoviesAsync(string section, int count)
        {
            var key = $"tmdb_home_{section}_{count}";
            if (_cache.TryGetValue<IEnumerable<TmdbMovieDto>>(key, out var cachedList))
            {
                return cachedList!;
            }

            string endpoint = section?.ToLower() switch
            {
                "lancamentos" => "movie/now_playing",
                "destaque" => "movie/top_rated",
                "maisassistidos" => "movie/popular",
                _ => "movie/popular"
            };

            var url = $"{endpoint}?api_key={_apiKey}&language=pt-BR&page=1";
            using var resp = await _http.GetAsync(url);
            resp.EnsureSuccessStatusCode();
            using var stream = await resp.Content.ReadAsStreamAsync();

            var doc = await JsonSerializer.DeserializeAsync<TmdbListResponse>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            var results = doc?.Results ?? Array.Empty<TmdbResult>();

            var list = results.Take(count).Select(r => new TmdbMovieDto
            {
                Codigo = r.Id,
                Titulo = r.Title ?? r.Name ?? "",
                SinopseCurta = r.Overview,
                PosterUrl = string.IsNullOrEmpty(r.PosterPath) ? null : (_imageBase + r.PosterPath),
                DataLancamento = r.ReleaseDate
            }).ToList();

            // cache por 30 minutos (ajuste se necessário)
            _cache.Set(key, list, TimeSpan.FromMinutes(30));

            return list;
        }

        public async Task<TmdbMovieDetailDto?> GetMovieDetailsAsync(int tmdbId)
        {
            var key = $"tmdb_details_{tmdbId}";
            if (_cache.TryGetValue<TmdbMovieDetailDto>(key, out var cachedDetail))
            {
                return cachedDetail!;
            }

            var url = $"movie/{tmdbId}?api_key={_apiKey}&language=pt-BR&append_to_response=videos";
            using var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode) return null;
            using var stream = await resp.Content.ReadAsStreamAsync();

            var doc = await JsonSerializer.DeserializeAsync<TmdbDetailResponse>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (doc == null) return null;

            var videos = (doc.Videos?.Results ?? Array.Empty<TmdbVideoResult>())
                .Select(v => new TmdbVideoDto { Id = v.Id, Key = v.Key, Site = v.Site, Type = v.Type, Name = v.Name })
                .ToList();

            var detail = new TmdbMovieDetailDto
            {
                Codigo = doc.Id,
                Titulo = doc.Title ?? doc.Name ?? "",
                Sinopse = doc.Overview,
                PosterUrl = string.IsNullOrEmpty(doc.PosterPath) ? null : (_imageBase + doc.PosterPath),
                DataLancamento = doc.ReleaseDate,
                Videos = videos
            };

            // cache por 60 minutos (ajuste conforme necessidade)
            _cache.Set(key, detail, TimeSpan.FromMinutes(60));

            return detail;
        }

        public async Task<IEnumerable<TmdbMovieDto>> SearchMoviesAsync(string query, int count)
        {
            var key = $"tmdb_search_{query}_{count}";
            if (_cache.TryGetValue<IEnumerable<TmdbMovieDto>>(key, out var cachedSearch))
            {
                return cachedSearch!;
            }

            var url = $"search/movie?api_key={_apiKey}&language=pt-BR&query={Uri.EscapeDataString(query)}&page=1";
            using var resp = await _http.GetAsync(url);
            resp.EnsureSuccessStatusCode();
            using var stream = await resp.Content.ReadAsStreamAsync();

            var doc = await JsonSerializer.DeserializeAsync<TmdbListResponse>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            var results = doc?.Results ?? Array.Empty<TmdbResult>();

            var list = results.Take(count).Select(r => new TmdbMovieDto
            {
                Codigo = r.Id,
                Titulo = r.Title ?? r.Name ?? "",
                SinopseCurta = r.Overview,
                PosterUrl = string.IsNullOrEmpty(r.PosterPath) ? null : (_imageBase + r.PosterPath),
                DataLancamento = r.ReleaseDate
            }).ToList();

            // cache de buscas por 15 minutos (ajuste conforme uso)
            _cache.Set(key, list, TimeSpan.FromMinutes(15));

            return list;
        }

        private class TmdbListResponse { public TmdbResult[] Results { get; set; } = Array.Empty<TmdbResult>(); }
        private class TmdbResult { public int Id { get; set; } public string? Title { get; set; } public string? Name { get; set; } public string? Overview { get; set; } public string? PosterPath { get; set; } public string? ReleaseDate { get; set; } }
        private class TmdbDetailResponse { public int Id { get; set; } public string? Title { get; set; } public string? Name { get; set; } public string? Overview { get; set; } public string? PosterPath { get; set; } public string? ReleaseDate { get; set; } public TmdbVideos? Videos { get; set; } }
        private class TmdbVideos { public TmdbVideoResult[] Results { get; set; } = Array.Empty<TmdbVideoResult>(); }
        private class TmdbVideoResult { public string? Id { get; set; } public string? Key { get; set; } public string? Site { get; set; } public string? Type { get; set; } public string? Name { get; set; } }
    }
}