using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PipocaResenha.Data;
using PipocaResenha.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace PipocaResenha.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            // 1. Lançamentos: Filmes em cartaz, ordenados por data de lançamento (mais recentes)
            var lancamentos = await _db.Filmes
                .Where(f => f.EmCartaz)
                .OrderByDescending(f => f.DataLancamento)
                .Take(10)
                .ToListAsync();

            // 2. Top Bem Avaliados: Baseado na média das notas dos reviews
            var topAvaliados = await _db.Filmes
                .Include(f => f.Reviews)
                .OrderByDescending(f => f.Reviews.Any() ? f.Reviews.Average(r => r.Nota) : 0)
                .Take(10)
                .ToListAsync();

            // 3. Mais Comentados: Baseado na quantidade de reviews (simulando "Mais Assistidos")
            var maisComentados = await _db.Filmes
                .Include(f => f.Reviews)
                .OrderByDescending(f => f.Reviews.Count())
                .Take(10)
                .ToListAsync();

            var viewModel = new HomeViewModel
            {
                Lancamentos = lancamentos,
                TopBemAvaliados = topAvaliados,
                MaisComentados = maisComentados
            };

            return View(viewModel);
        }

        public IActionResult Sobre() => View();
        public IActionResult Contato() => View();

        // Adicionar página de erro para produção
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View();
    }
}