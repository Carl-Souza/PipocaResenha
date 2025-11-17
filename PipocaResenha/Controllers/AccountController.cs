using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PipocaResenha.Data;
using PipocaResenha.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PipocaResenha.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _db;
        public AccountController(ApplicationDbContext db) { _db = db; }

        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(string name, string email, string password)
        {
            if (await _db.Usuarios.AnyAsync(u => u.Email == email))
                return BadRequest("Email já cadastrado.");

            var usuario = new Usuarios
            {
                Nome = name,
                Email = email,
                PasswordHash = Hash(password)
            };

            _db.Usuarios.Add(usuario);
            await _db.SaveChangesAsync();
            return RedirectToAction("Login");
        }

        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var usuario = await _db.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
            if (usuario == null || usuario.PasswordHash != Hash(password))
                return BadRequest("Credenciais inválidas.");

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim("Codigo", usuario.Codigo.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        private string Hash(string input)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToBase64String(bytes);
        }
    }
}