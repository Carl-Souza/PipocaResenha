using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PipocaResenha.Data;
using System.Linq;
using System.Threading.Tasks;

namespace PipocaResenha.Controllers.Api
{
    [ApiController]
    [Route("api/filmes")]
    public class FilmesApiController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public FilmesApiController(ApplicationDbContext db) { _db = db; }

        [HttpGet("tmdb")]
        public async Task<IActionResult> GetSection(string section = "Lancamentos", int count = 12)
        {
            var q = _db.Filmes.Include(f => f.Reviews).AsQueryable();

            switch (section)
            {
                case "Destaque":
                    q = q.OrderByDescending(f => f.Reviews.Any() ? f.Reviews.Average(r => (double)r.Nota) : 0);
                    break;
                case "MaisAssistidos":
                    q = q.OrderByDescending(f => f.Reviews.Count);
                    break;
                default: // Lancamentos
                    q = q.Where(f => f.EmCartaz).OrderByDescending(f => f.DataLancamento);
                    break;
            }

            var list = await q.Take(count).Select(f => new
            {
                codigo = f.Codigo,
                titulo = f.Titulo,
                sinopseCurta = f.SinopseCurta,
                posterUrl = f.PosterUrl,
                detailsUrl = Url.Action("Details", "Filmes", new { codigo = f.Codigo })
            }).ToListAsync();

            return Ok(list);
        }

        [HttpGet("tmdb/search")]
        public async Task<IActionResult> Search(string query, int count = 40)
        {
            if (string.IsNullOrWhiteSpace(query)) return BadRequest();
            var list = await _db.Filmes
                .Where(f => f.Titulo.Contains(query))
                .Take(count)
                .Select(f => new
                {
                    codigo = f.Codigo,
                    titulo = f.Titulo,
                    sinopseCurta = f.SinopseCurta,
                    posterUrl = f.PosterUrl,
                    detailsUrl = Url.Action("Details", "Filmes", new { codigo = f.Codigo })
                })
                .ToListAsync();
            return Ok(list);
        }
    }
}