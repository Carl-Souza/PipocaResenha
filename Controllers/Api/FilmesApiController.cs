using Microsoft.AspNetCore.Mvc;
using PipocaResenha.Services;
using System.Linq;
using System.Threading.Tasks;

namespace PipocaResenha.Controllers.Api
{
    [Route("api/filmes")]
    [ApiController]
    public class FilmesApiController : ControllerBase
    {
        private readonly ITmdbService _tmdb;
        public FilmesApiController(ITmdbService tmdb) => _tmdb = tmdb;

        // Lista principal (usada pelos carrosséis)
        [HttpGet("tmdb")]
        public async Task<IActionResult> TmdbHome([FromQuery] string section = "Lancamentos", [FromQuery] int count = 10)
        {
            var list = await _tmdb.GetHomeMoviesAsync(section, count);
            var result = list.Select(m => new {
                codigo = m.Codigo,
                titulo = m.Titulo,
                sinopseCurta = m.SinopseCurta,
                posterUrl = m.PosterUrl,
                detailsUrl = $"/Filmes/Details?tmdbId={m.Codigo}" // rota local que pode abrir detalhe via TMDB id
            }).ToList();

            return Ok(result);
        }

        // Pesquisa livre
        [HttpGet("tmdb/search")]
        public async Task<IActionResult> Search([FromQuery] string query, [FromQuery] int count = 5)
        {
            if (string.IsNullOrWhiteSpace(query)) return BadRequest("query é obrigatório");
            var list = await _tmdb.SearchMoviesAsync(query, count);
            return Ok(list.Select(m => new { codigo = m.Codigo, titulo = m.Titulo, posterUrl = m.PosterUrl, sinopseCurta = m.SinopseCurta }));
        }

        // Detalhes + trailers (por tmdb id)
        [HttpGet("tmdb/details/{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var d = await _tmdb.GetMovieDetailsAsync(id);
            if (d == null) return NotFound();
            return Ok(new {
                codigo = d.Codigo,
                titulo = d.Titulo,
                sinopse = d.Sinopse,
                posterUrl = d.PosterUrl,
                dataLancamento = d.DataLancamento,
                videos = d.Videos
            });
        }
    }
}       