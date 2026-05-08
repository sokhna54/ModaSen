using AppVetements.Data;
using AppVetements.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AppVetements.Controllers
{
    public class ProduitController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

        public ProduitController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        // Liste des produits
        public async Task<IActionResult> Index()
        {
            var produits = await _db.Produits.Include(p => p.Categorie).ToListAsync();
            return View(produits);
        }

        // Formulaire ajout
        public IActionResult Ajouter()
        {
            ViewBag.Categories = new SelectList(_db.Categories, "ID", "NomCategorie");
            return View();
        }

        // Enregistrer ajout
        [HttpPost]
        public async Task<IActionResult> Ajouter(Produit produit, IFormFile? image)
        {
            if (image != null)
            {
                string dossier = Path.Combine(_env.WebRootPath, "images/produits");
                Directory.CreateDirectory(dossier);
                string nomFichier = Guid.NewGuid() + Path.GetExtension(image.FileName);
                string chemin = Path.Combine(dossier, nomFichier);
                using var stream = new FileStream(chemin, FileMode.Create);
                await image.CopyToAsync(stream);
                produit.ImageProduit = "/images/produits/" + nomFichier;
            }

            _db.Produits.Add(produit);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // Formulaire modification
        public async Task<IActionResult> Modifier(int id)
        {
            var produit = await _db.Produits.FindAsync(id);
            ViewBag.Categories = new SelectList(_db.Categories, "ID", "NomCategorie", produit.CategorieID);
            return View(produit);
        }

        // Enregistrer modification
        [HttpPost]
        public async Task<IActionResult> Modifier(Produit produit, IFormFile? image)
        {
            if (image != null)
            {
                string dossier = Path.Combine(_env.WebRootPath, "images/produits");
                Directory.CreateDirectory(dossier);
                string nomFichier = Guid.NewGuid() + Path.GetExtension(image.FileName);
                string chemin = Path.Combine(dossier, nomFichier);
                using var stream = new FileStream(chemin, FileMode.Create);
                await image.CopyToAsync(stream);
                produit.ImageProduit = "/images/produits/" + nomFichier;
            }

            _db.Produits.Update(produit);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // Supprimer
        public async Task<IActionResult> Supprimer(int id)
        {
            var produit = await _db.Produits.FindAsync(id);
            _db.Produits.Remove(produit);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}