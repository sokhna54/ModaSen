using AppVetements.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppVetements.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;

        public HomeController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            // Récupérer seulement les produits en promo
            var produitsEnPromo = await _db.Produits
                .Include(p => p.Categorie)
                .Where(p => p.Remise > 0)
                .ToListAsync();

            ViewBag.ProduitsEnPromo = produitsEnPromo;
            return View();
        }
    }
}