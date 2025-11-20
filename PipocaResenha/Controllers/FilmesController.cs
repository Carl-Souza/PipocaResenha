    using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PipocaResenha.Data;
using PipocaResenha.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PipocaResenha.Controllers
{
    public class FilmesController : Controller
    {
        private readonly ApplicationDbContext _db;
        public FilmesController(ApplicationDbContext db) { _db = db; }

        public async Task<IActionResult> Index()
        {
            var lancamentos = await _db.Filmes
                .Where(m => m.EmCartaz)
                .OrderByDescending(m => m.DataLancamento)
                .Take(12)
                .ToListAsync();

            var top10 = await _db.Filmes
                .OrderByDescending(m => m.Reviews.Average(r => (double?)r.Nota) ?? 0)
                .Take(10)
                .ToListAsync();

            ViewBag.Top10 = top10;
            return View(lancamentos);
        }

        public async Task<IActionResult> Details(int codigo)
        {
            var movie = await _db.Filmes
                .Include(m => m.Reviews)
                .FirstOrDefaultAsync(m => m.Codigo == codigo);

            if (movie == null) return NotFound();
            return View(movie);
        }

        public async Task<IActionResult> ListaCinemasParaFilmes(int codigoFilme, int page = 1)
        {
            int pageSize = 5;

            var cinemas = await _db.FilmesCinemas
                .Where(mc => mc.CodigoFilme == codigoFilme)
                .Include(mc => mc.Cinema)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(mc => new {
                    nome = mc.Cinema.Nome,
                    cidade = mc.Cinema.Cidade,
                    endereco = mc.Cinema.Endereco
                })
                .ToListAsync();

            return Json(cinemas);
        }

        public async Task<IActionResult> All(string search, string age, string cidade)
        {
            var query = _db.Filmes.Include(m => m.FilmesCinemas).AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(m => m.Titulo.Contains(search));

            if (!string.IsNullOrEmpty(age))
                query = query.Where(m => m.FaixaEtaria == age);

            if (!string.IsNullOrEmpty(cidade))
                query = query.Where(m => m.FilmesCinemas.Any(c => c.Cinema.Cidade == cidade));

            return View(await query.ToListAsync());
        }
    }
}