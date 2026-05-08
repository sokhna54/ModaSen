using AppVetements.Data;
using AppVetements.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppVetements.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _db;

        public AdminController(AppDbContext db)
        {
            _db = db;
        }

        // Vérifier si admin
        private bool EstAdmin()
        {
            return HttpContext.Session.GetString("AdminNom") != null;
        }

        // Dashboard
        public async Task<IActionResult> Index()
        {
            if (!EstAdmin()) return RedirectToAction("Login");

            ViewBag.TotalProduits = await _db.Produits.CountAsync();
            ViewBag.TotalClients = await _db.Clients.CountAsync();
            ViewBag.TotalCommandes = await _db.Commandes.CountAsync();
            ViewBag.TotalRevenu = await _db.Commandes.SumAsync(c => c.Total);

            var dernieresCommandes = await _db.Commandes
                .Include(c => c.Client)
                .OrderByDescending(c => c.DateCommande)
                .Take(5)
                .ToListAsync();

            return View(dernieresCommandes);
        }

        // Login Admin
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string login, string motDePasse)
        {
            // Identifiants admin fixes
            if (login == "admin" && motDePasse == "Admin@123")
            {
                HttpContext.Session.SetString("AdminNom", "Administrateur");
                return RedirectToAction("Index");
            }

            ViewBag.Erreur = "Identifiants incorrects !";
            return View();
        }

        // Déconnexion admin
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("AdminNom");
            return RedirectToAction("Index", "Home"); // ✅ Redirige vers accueil
        }

        // Gestion commandes
        public async Task<IActionResult> Commandes()
        {
            if (!EstAdmin()) return RedirectToAction("Login");

            var commandes = await _db.Commandes
                .Include(c => c.Client)
                .Include(c => c.LignesCommande)
                .ThenInclude(l => l.Produit)
                .OrderByDescending(c => c.DateCommande)
                .ToListAsync();

            return View(commandes);
        }

        // Changer statut commande
        public async Task<IActionResult> ChangerStatut(int id, string statut)
        {
            if (!EstAdmin()) return RedirectToAction("Login");

            var commande = await _db.Commandes
                .Include(c => c.Client)
                .FirstOrDefaultAsync(c => c.ID == id);

            if (commande != null)
            {
                commande.Statut = statut;
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("Commandes");
        }

        // Gestion produits
        public async Task<IActionResult> Produits()
        {
            if (!EstAdmin()) return RedirectToAction("Login");

            var produits = await _db.Produits
                .Include(p => p.Categorie)
                .ToListAsync();

            return View(produits);
        }

        // Gestion clients
        public async Task<IActionResult> Clients()
        {
            if (!EstAdmin()) return RedirectToAction("Login");

            var clients = await _db.Clients.ToListAsync();
            return View(clients);
        }
    }
}