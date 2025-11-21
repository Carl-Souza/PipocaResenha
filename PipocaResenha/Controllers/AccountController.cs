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

            if (await _db.Usuario.AnyAsync(u => u.Email == email))
            {
                TempData["MensagemErro"] = "Este e-mail já está cadastrado! Tente fazer login.";
                return View(); 
            }

            var usuario = new Usuarios
            {
                Nome = name,
                Email = email,
                PasswordHash = password 
            };

            _db.Usuario.Add(usuario);
            await _db.SaveChangesAsync();

            TempData["MensagemSucesso"] = "Conta criada com sucesso! Faça login.";
            return RedirectToAction("Login");
        }

        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var usuario = await _db.Usuario.FirstOrDefaultAsync(u => u.Email == email);

            if (usuario == null || usuario.PasswordHash != password)
            {
                TempData["MensagemErro"] = "E-mail ou senha incorretos. Verifique suas credenciais.";
                return View(); 
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim("Codigo", usuario.Codigo.ToString())
            };

            var identity = new ClaimsIdentity(claims, "CookieAuth");
            await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(identity));

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}