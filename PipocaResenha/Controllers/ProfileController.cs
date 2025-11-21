using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using PipocaResenha.Data;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Claims; // Necessário para ler o User.FindFirst

namespace PipocaResenha.Controllers
{
    [Authorize] // Garante que só quem está logado acessa
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ProfileController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: Carrega a tela do perfil
        public async Task<IActionResult> Index()
        {
            // Pega o ID do usuário logado através do Cookie
            var claimCodigo = User.FindFirst("Codigo");
            if (claimCodigo == null) return RedirectToAction("Login", "Account");

            int codigoUsuario = int.Parse(claimCodigo.Value);

            var usuario = await _db.Usuario
                .Include(u => u.Reviews)
                .FirstOrDefaultAsync(u => u.Codigo == codigoUsuario);

            if (usuario == null) return NotFound();

            return View(usuario);
        }

        // POST: Processa a edição dos dados (Backend da edição)
        [HttpPost]
        [ValidateAntiForgeryToken] // Segurança contra ataques CSRF
        public async Task<IActionResult> EditProfile(string nome, string photoUrl)
        {
            // 1. Identifica quem é o usuário logado
            var claimCodigo = User.FindFirst("Codigo");
            if (claimCodigo == null) return RedirectToAction("Login", "Account");

            int codigoUsuario = int.Parse(claimCodigo.Value);

            // 2. Busca o usuário no banco
            var usuario = await _db.Usuario.FindAsync(codigoUsuario);

            if (usuario == null) return NotFound();

            // 3. Atualiza os dados
            // Se o campo nome vier vazio (por inspeção de elemento), mantém o antigo por segurança
            if (!string.IsNullOrWhiteSpace(nome))
            {
                usuario.Nome = nome;
            }

            // Atualiza a foto (pode ser vazia se o usuário quiser remover)
            usuario.PhotoUrl = photoUrl;

            // 4. Salva as alterações no banco de dados
            _db.Usuario.Update(usuario);
            await _db.SaveChangesAsync();

            // 5. Atualiza o cookie de autenticação para refletir o novo nome imediatamente (Opcional, mas recomendado)
            // Para simplificar aqui, apenas redirecionamos. O nome no header atualizará no próximo login ou se recriarmos o cookie.

            return RedirectToAction("Index");
        }

        // POST: Excluir Conta
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAccount()
        {
            var claimCodigo = User.FindFirst("Codigo");
            if (claimCodigo == null) return RedirectToAction("Login", "Account");

            int codigoUsuario = int.Parse(claimCodigo.Value);

            var usuario = await _db.Usuario
                .Include(u => u.Reviews) // Traz os reviews junto para apagar em cascata
                .FirstOrDefaultAsync(u => u.Codigo == codigoUsuario);

            if (usuario != null)
            {
                // Remove reviews primeiro (se o banco não tiver Cascade Delete configurado)
                if (usuario.Reviews != null && usuario.Reviews.Any())
                {
                    _db.Reviews.RemoveRange(usuario.Reviews);
                }

                // Remove o usuário
                _db.Usuario.Remove(usuario);
                await _db.SaveChangesAsync();
            }

            // Desloga o usuário
            await Microsoft.AspNetCore.Authentication.AuthenticationHttpContextExtensions.SignOutAsync(HttpContext);

            return RedirectToAction("Index", "Home");
        }

        // API: Paginação dos reviews (Já implementamos anteriormente)
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