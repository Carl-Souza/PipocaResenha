using System.Collections.Generic;
using System.Threading.Tasks;

namespace PipocaResenha.Services
{
    public interface ITmdbService
    {
        Task<IEnumerable<TmdbMovieDto>> GetHomeMoviesAsync(string section, int count);
        Task<TmdbMovieDetailDto?> GetMovieDetailsAsync(int tmdbId);
        Task<IEnumerable<TmdbMovieDto>> SearchMoviesAsync(string query, int count);
    }

    public class TmdbMovieDto
    {
        public int Codigo { get; set; } // TMDB id
        public string Titulo { get; set; } = "";
        public string? SinopseCurta { get; set; }
        public string? PosterUrl { get; set; }
        public string? DataLancamento { get; set; }
    }

    public class TmdbMovieDetailDto
    {
        public int Codigo { get; set; }
        public string Titulo { get; set; } = "";
        public string? Sinopse { get; set; }
        public string? PosterUrl { get; set; }
        public string? DataLancamento { get; set; }
        public IEnumerable<TmdbVideoDto> Videos { get; set; } = new List<TmdbVideoDto>();
    }

    public class TmdbVideoDto
    {
        public string? Id { get; set; }
        public string? Key { get; set; }
        public string? Site { get; set; }
        public string? Type { get; set; }
        public string? Name { get; set; }
    }
}