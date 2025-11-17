using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PipocaResenha.Data;
using PipocaResenha.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PipocaResenha.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _db;
        public MoviesController(ApplicationDbContext db) { _db = db; }

        public async Task<IActionResult> Index()
        {
            var lancamentos = await _db.Movies
                .Where(m => m.IsNowShowing)
                .OrderByDescending(m => m.ReleaseDate)
                .Take(12)
                .ToListAsync();

            var top10 = await _db.Movies
                .OrderByDescending(m => m.Reviews.Average(r => (double?)r.Rating) ?? 0)
                .Take(10)
                .ToListAsync();

            ViewBag.Top10 = top10;
            return View(lancamentos);
        }

        public async Task<IActionResult> Details(int id)
        {
            var movie = await _db.Movies
                .Include(m => m.Reviews)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null) return NotFound();
            return View(movie);
        }

        public async Task<IActionResult> ListCinemasForMovie(int movieId, int page = 1)
        {
            int pageSize = 5;

            var cinemas = await _db.MovieCinemas
                .Where(mc => mc.MovieId == movieId)
                .Include(mc => mc.Cinema)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(mc => new {
                    name = mc.Cinema.Name,
                    city = mc.Cinema.City,
                    address = mc.Cinema.Address
                })
                .ToListAsync();

            return Json(cinemas);
        }

        public async Task<IActionResult> All(string search, string age, string city)
        {
            var query = _db.Movies.Include(m => m.MovieCinemas).AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(m => m.Title.Contains(search));

            if (!string.IsNullOrEmpty(age))
                query = query.Where(m => m.AgeRating == age);

            if (!string.IsNullOrEmpty(city))
                query = query.Where(m => m.MovieCinemas.Any(c => c.Cinema.City == city));

            return View(await query.ToListAsync());
        }
    }
}