using AppVetements.Data;
using AppVetements.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AppVetements.Controllers
{
    public class PanierController : Controller
    {
        private readonly AppDbContext _db;
        private const string CLE_PANIER = "Panier";

        public PanierController(AppDbContext db)
        {
            _db = db;
        }

        // Récupérer le panier depuis la session
        private List<PanierItem> GetPanier()
        {
            var json = HttpContext.Session.GetString(CLE_PANIER);
            return json == null ? new List<PanierItem>() : JsonSerializer.Deserialize<List<PanierItem>>(json);
        }

        // Sauvegarder le panier dans la session
        private void SavePanier(List<PanierItem> panier)
        {
            HttpContext.Session.SetString(CLE_PANIER, JsonSerializer.Serialize(panier));
        }

        // Afficher le panier
        public IActionResult Index()
        {
            var panier = GetPanier();
            return View(panier);
        }

        // Ajouter au panier
        public async Task<IActionResult> Ajouter(int id)
        {
            var produit = await _db.Produits.FindAsync(id);
            if (produit == null) return RedirectToAction("Index", "Boutique");

            var panier = GetPanier();
            var item = panier.FirstOrDefault(p => p.ProduitID == id);

            if (item != null)
            {
                item.Quantite++;
            }
            else
            {
                panier.Add(new PanierItem
                {
                    ProduitID = produit.ID,
                    NomProduit = produit.NomProduit,
                    Prix = produit.EnPromo ? produit.PrixPromo : produit.Prix,
                    Quantite = 1,
                    ImageProduit = produit.ImageProduit
                });
            }

            SavePanier(panier);
            return RedirectToAction("Index");
        }

        // Supprimer du panier
        public IActionResult Supprimer(int id)
        {
            var panier = GetPanier();
            var item = panier.FirstOrDefault(p => p.ProduitID == id);
            if (item != null) panier.Remove(item);
            SavePanier(panier);
            return RedirectToAction("Index");
        }

        // Vider le panier
        public IActionResult Vider()
        {
            HttpContext.Session.Remove(CLE_PANIER);
            return RedirectToAction("Index");
        }

        // Augmenter quantité
        public IActionResult Augmenter(int id)
        {
            var panier = GetPanier();
            var item = panier.FirstOrDefault(p => p.ProduitID == id);
            if (item != null) item.Quantite++;
            SavePanier(panier);
            return RedirectToAction("Index");
        }

        // Diminuer quantité
        public IActionResult Diminuer(int id)
        {
            var panier = GetPanier();
            var item = panier.FirstOrDefault(p => p.ProduitID == id);
            if (item != null)
            {
                item.Quantite--;
                if (item.Quantite <= 0) panier.Remove(item);
            }
            SavePanier(panier);
            return RedirectToAction("Index");
        }
    }
}