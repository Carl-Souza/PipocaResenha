using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PipocaResenha.Data;
using PipocaResenha.Models;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.Linq;

namespace PipocaResenha.Controllers
{
    [Authorize]
    public class ReviewsController : Controller
    {
        private readonly ApplicationDbContext _db;
        public ReviewsController(ApplicationDbContext db) { _db = db; }

        [HttpPost]
        public async Task<IActionResult> Create(int codigoFilme, byte nota, string text)
        {
            int codigoUsuario = int.Parse(User.FindFirst("Codigo").Value);

            bool exists = await _db.Reviews
                .AnyAsync(r => r.CodigoFilme == codigoFilme && r.CodigoUsuario == codigoUsuario);

            if (exists)
                return BadRequest("Você já avaliou este filme.");

            var review = new Review
            {
                CodigoFilme = codigoFilme,
                CodigoUsuario = codigoUsuario,
                Nota = nota,
                TextoReview = text
            };

            _db.Reviews.Add(review);
            await _db.SaveChangesAsync();

            return RedirectToAction("Details", "Movies", new { codigo = codigoFilme });
        }

        public async Task<IActionResult> Edit(int codigo)
        {
            int codigoUsuario = int.Parse(User.FindFirst("Codigo").Value);
            var review = await _db.Reviews.FirstOrDefaultAsync(r => r.Codigo == codigo && r.CodigoUsuario == codigoUsuario);

            if (review == null) return Unauthorized();
            return View(review);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Review model)
        {
            int codigoUsuario = int.Parse(User.FindFirst("Codigo").Value);
            var review = await _db.Reviews.FirstOrDefaultAsync(r => r.Codigo == model.Codigo && r.CodigoUsuario == codigoUsuario);

            if (review == null) return Unauthorized();

            review.Nota = model.Nota;
            review.TextoReview = model.TextoReview;

            await _db.SaveChangesAsync();
            return RedirectToAction("Details", "Movies", new { codigo = review.CodigoFilme });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int codigo)
        {
            int codigoUsuario = int.Parse(User.FindFirst("Codigo").Value);
            var review = await _db.Reviews.FirstOrDefaultAsync(r => r.Codigo == codigo && r.CodigoUsuario == codigoUsuario  );

            if (review == null) return Unauthorized();

            _db.Reviews.Remove(review);
            await _db.SaveChangesAsync();

            return Ok();
        }
    }
}