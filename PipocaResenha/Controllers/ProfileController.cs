using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using PipocaResenha.Data;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authentication;

namespace PipocaResenha.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _db;
        public ProfileController(ApplicationDbContext db) { _db = db; }

        public async Task<IActionResult> Index()
        {
            int userId = int.Parse(User.FindFirst("Id").Value);

            var user = await _db.Users
                .Include(u => u.Reviews)
                .ThenInclude(r => r.Movie)
                .FirstOrDefaultAsync(u => u.Id == userId);

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(string name, string photoUrl)
        {
            int userId = int.Parse(User.FindFirst("Id").Value);
            var user = await _db.Users.FindAsync(userId);

            user.Name = name;
            user.PhotoUrl = photoUrl;

            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAccount()
        {
            int userId = int.Parse(User.FindFirst("Id").Value);

            var user = await _db.Users
                .Include(u => u.Reviews)
                .FirstOrDefaultAsync(u => u.Id == userId);

            _db.Reviews.RemoveRange(user.Reviews);
            _db.Users.Remove(user);

            await _db.SaveChangesAsync();
            await HttpContext.SignOutAsync(); 

            return RedirectToAction("Index", "Home");
        }
    }
}