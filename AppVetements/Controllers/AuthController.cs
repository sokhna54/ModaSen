using AppVetements.Data;
using AppVetements.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace AppVetements.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _db;

        public AuthController(AppDbContext db)
        {
            _db = db;
        }

        // Page Register
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(Client client)
        {
            // Vérifier si email existe déjà
            if (_db.Clients.Any(c => c.Email == client.Email))
            {
                ViewBag.Erreur = "Cet email est déjà utilisé !";
                return View();
            }

            // Hasher le mot de passe
            client.MotDePasse = HashMotDePasse(client.MotDePasse);
            _db.Clients.Add(client);
            await _db.SaveChangesAsync();
            return RedirectToAction("Login");
        }

        // Page Login
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string motDePasse)
        {
            string mdpHash = HashMotDePasse(motDePasse);
            var client = _db.Clients.FirstOrDefault(c => c.Email == email && c.MotDePasse == mdpHash);

            if (client == null)
            {
                ViewBag.Erreur = "Email ou mot de passe incorrect !";
                return View();
            }

            // Sauvegarder en session
            HttpContext.Session.SetInt32("ClientID", client.ID);
            HttpContext.Session.SetString("ClientNom", client.NomComplet);

            return RedirectToAction("Index", "Home");
        }

        // Déconnexion
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // Hasher mot de passe
        private string HashMotDePasse(string motDePasse)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(motDePasse));
            return Convert.ToBase64String(bytes);
        }
    }
}