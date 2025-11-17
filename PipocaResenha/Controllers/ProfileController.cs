using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using PipocaResenha.Data;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using PipocaResenha.Models;

namespace PipocaResenha.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _db;
        public ProfileController(ApplicationDbContext db) { _db = db; }

        public async Task<IActionResult> Index()
        {
            int codigoUsuario = int.Parse(User.FindFirst("Codigo").Value);
            
            var usuario = await _db.Usuarios
                .Include(u => u.Reviews)
                .ThenInclude(r => r.Filme)
                .FirstOrDefaultAsync(u => u.Codigo == codigoUsuario);

            return View(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(string nome, string photoUrl)
        {
            int codigoUsuario = int.Parse(User.FindFirst("Codigo").Value);
            var usuario = await _db.Usuarios.FindAsync(codigoUsuario);

            usuario.Nome = nome;
            usuario.PhotoUrl = photoUrl;

            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAccount()
        {
            int codigoUsuario = int.Parse(User.FindFirst("Codigo").Value);

            var usuario = await _db.Usuarios
                .Include(u => u.Reviews)
                .FirstOrDefaultAsync(u => u.Codigo == codigoUsuario);

            _db.Reviews.RemoveRange(usuario.Reviews);
            _db.Usuarios.Remove(usuario);

            await _db.SaveChangesAsync();
            await HttpContext.SignOutAsync(); 

            return RedirectToAction("Index", "Home");
        }
    }
}