using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using PipocaResenha.Data;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Claims; 

namespace PipocaResenha.Controllers
{
    [Authorize] 
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ProfileController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var claimCodigo = User.FindFirst("Codigo");
            if (claimCodigo == null) return RedirectToAction("Login", "Account");

            int codigoUsuario = int.Parse(claimCodigo.Value);

            var usuario = await _db.Usuario
                .Include(u => u.Reviews)
                .FirstOrDefaultAsync(u => u.Codigo == codigoUsuario);

            if (usuario == null) return NotFound();

            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> EditProfile(string nome, string photoUrl)
        {

            var claimCodigo = User.FindFirst("Codigo");
            if (claimCodigo == null) return RedirectToAction("Login", "Account");

            int codigoUsuario = int.Parse(claimCodigo.Value);

            var usuario = await _db.Usuario.FindAsync(codigoUsuario);

            if (usuario == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(nome))
            {
                usuario.Nome = nome;
            }

            usuario.PhotoUrl = photoUrl;

            _db.Usuario.Update(usuario);
            await _db.SaveChangesAsync();


            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAccount()
        {
            var claimCodigo = User.FindFirst("Codigo");
            if (claimCodigo == null) return RedirectToAction("Login", "Account");

            int codigoUsuario = int.Parse(claimCodigo.Value);

            var usuario = await _db.Usuario
                .Include(u => u.Reviews) 
                .FirstOrDefaultAsync(u => u.Codigo == codigoUsuario);

            if (usuario != null)
            {

                if (usuario.Reviews != null && usuario.Reviews.Any())
                {
                    _db.Reviews.RemoveRange(usuario.Reviews);
                }

                _db.Usuario.Remove(usuario);
                await _db.SaveChangesAsync();
            }

            await Microsoft.AspNetCore.Authentication.AuthenticationHttpContextExtensions.SignOutAsync(HttpContext);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> GetMyReviews(int page = 1)
        {
            int pageSize = 5;
            var claimCodigo = User.FindFirst("Codigo");
            if (claimCodigo == null) return Unauthorized();

            int codigoUsuario = int.Parse(claimCodigo.Value);

            var reviews = await _db.Reviews
                .Where(r => r.CodigoUsuario == codigoUsuario)
                .Include(r => r.Filmes)
                .OrderByDescending(r => r.Codigo)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new {
                    tituloFilme = r.Filmes.Titulo,
                    texto = r.TextoReview,
                    nota = r.Nota,
                    codigo = r.Codigo
                })
                .ToListAsync();

            return Json(reviews);
        }
    }
}