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
        public async Task<IActionResult> Create(int movieId, byte rating, string text)
        {
            int userId = int.Parse(User.FindFirst("Id").Value);

            bool exists = await _db.Reviews
                .AnyAsync(r => r.MovieId == movieId && r.UserId == userId);

            if (exists)
                return BadRequest("Você já avaliou este filme.");

            var review = new Review
            {
                MovieId = movieId,
                UserId = userId,
                Rating = rating,
                Text = text
            };

            _db.Reviews.Add(review);
            await _db.SaveChangesAsync();

            return RedirectToAction("Details", "Movies", new { id = movieId });
        }

        public async Task<IActionResult> Edit(int id)
        {
            int userId = int.Parse(User.FindFirst("Id").Value);
            var review = await _db.Reviews.FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

            if (review == null) return Unauthorized();
            return View(review);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Review model)
        {
            int userId = int.Parse(User.FindFirst("Id").Value);
            var review = await _db.Reviews.FirstOrDefaultAsync(r => r.Id == model.Id && r.UserId == userId);

            if (review == null) return Unauthorized();

            review.Rating = model.Rating;
            review.Text = model.Text;

            await _db.SaveChangesAsync();
            return RedirectToAction("Details", "Movies", new { id = review.MovieId });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            int userId = int.Parse(User.FindFirst("Id").Value);
            var review = await _db.Reviews.FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

            if (review == null) return Unauthorized();

            _db.Reviews.Remove(review);
            await _db.SaveChangesAsync();

            return Ok();
        }
    }
}