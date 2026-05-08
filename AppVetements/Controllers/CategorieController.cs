using AppVetements.Data;
using AppVetements.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppVetements.Controllers
{
    public class CategorieController : Controller
    {
        private readonly AppDbContext _db;

        public CategorieController(AppDbContext db)
        {
            _db = db;
        }

        // Liste
        public async Task<IActionResult> Index()
        {
            var categories = await _db.Categories.ToListAsync();
            return View(categories);
        }

        // Formulaire ajout
        public IActionResult Ajouter()
        {
            return View();
        }

        // Enregistrer ajout
        [HttpPost]
        public async Task<IActionResult> Ajouter(Categorie categorie)
        {
            _db.Categories.Add(categorie);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // Formulaire modification
        public async Task<IActionResult> Modifier(int id)
        {
            var categorie = await _db.Categories.FindAsync(id);
            return View(categorie);
        }

        // Enregistrer modification
        [HttpPost]
        public async Task<IActionResult> Modifier(Categorie categorie)
        {
            _db.Categories.Update(categorie);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // Supprimer
        public async Task<IActionResult> Supprimer(int id)
        {
            var categorie = await _db.Categories.FindAsync(id);
            _db.Categories.Remove(categorie);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}