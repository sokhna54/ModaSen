using AppVetements.Data;
using AppVetements.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppVetements.Controllers
{
    public class BoutiqueController : Controller
    {
        private readonly AppDbContext _db;

        public BoutiqueController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(int? categorie, string? recherche)
        {
            var produits = _db.Produits.Include(p => p.Categorie).AsQueryable();

            if (categorie.HasValue)
                produits = produits.Where(p => p.CategorieID == categorie);

            if (!string.IsNullOrEmpty(recherche))
                produits = produits.Where(p => p.NomProduit.Contains(recherche));

            ViewBag.Categories = await _db.Categories.ToListAsync();
            ViewBag.Recherche = recherche;
            ViewBag.CategorieSelectionnee = categorie;

            return View(await produits.ToListAsync());
        }

        public async Task<IActionResult> Detail(int id)
        {
            var produit = await _db.Produits.Include(p => p.Categorie).FirstOrDefaultAsync(p => p.ID == id);
            return View(produit);
        }
    }
}