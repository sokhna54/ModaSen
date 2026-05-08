using AppVetements.Data;
using AppVetements.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using AppVetements.Helpers;
using Microsoft.Extensions.Configuration;
using AppVetements.Helpers;
namespace AppVetements.Controllers
{
    public class CommandeController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;
        private const string CLE_PANIER = "Panier";

        public CommandeController(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        private List<PanierItem> GetPanier()
        {
            var json = HttpContext.Session.GetString(CLE_PANIER);
            return json == null ? new List<PanierItem>() : JsonSerializer.Deserialize<List<PanierItem>>(json);
        }

        // Page passer commande
        public IActionResult Passer()
        {
            var clientID = HttpContext.Session.GetInt32("ClientID");
            if (clientID == null) return RedirectToAction("Login", "Auth");

            var panier = GetPanier();
            if (!panier.Any()) return RedirectToAction("Index", "Panier");

            var client = _db.Clients.Find(clientID);
            ViewBag.Panier = panier;
            ViewBag.Total = panier.Sum(p => p.Total);
            return View(client);
        }

        // Confirmer commande
        [HttpPost]
        public async Task<IActionResult> Confirmer(string adresseLivraison)
        {
            var clientID = HttpContext.Session.GetInt32("ClientID");
            if (clientID == null) return RedirectToAction("Login", "Auth");

            var panier = GetPanier();
            if (!panier.Any()) return RedirectToAction("Index", "Panier");

            // Créer la commande
            var commande = new Commande
            {
                ClientID = clientID.Value,
                DateCommande = DateTime.Now,
                Total = panier.Sum(p => p.Total),
                Statut = "En attente"
            };

            _db.Commandes.Add(commande);
            await _db.SaveChangesAsync();

            // Ajouter les lignes de commande
            foreach (var item in panier)
            {
                var ligne = new LigneCommande
                {
                    CommandeID = commande.ID,
                    ProduitID = item.ProduitID,
                    Quantite = item.Quantite,
                    PrixUnitaire = item.Prix
                };
                _db.LignesCommande.Add(ligne);

                // Diminuer le stock
                var produit = await _db.Produits.FindAsync(item.ProduitID);
                if (produit != null) produit.Stock -= item.Quantite;
            }

            await _db.SaveChangesAsync();

            // Vider le panier
            HttpContext.Session.Remove(CLE_PANIER);

            // Envoyer email
            var client = await _db.Clients.FindAsync(clientID);
            try
            {
                GMailer.GmailUsername = _config["EmailSettings:Email"];
                GMailer.GmailPassword = _config["EmailSettings:PasswordEmail"];

                string sujet = $"✅ Confirmation commande #{commande.ID} - ModaSen";
                string body = $@"
                <div style='font-family:Segoe UI; max-width:600px; margin:auto;'>
                    <div style='background:#1a1a2e; padding:30px; text-align:center;'>
                        <h1 style='color:#f0a500;'>👗 ModaSen</h1>
                    </div>
                    <div style='padding:30px; background:#f9f9f9;'>
                        <h2>Bonjour {client.NomComplet} !</h2>
                        <p>Votre commande a été confirmée avec succès.</p>
                        <div style='background:white; border-radius:15px; padding:20px;'>
                            <p><strong>Numéro :</strong> #{commande.ID}</p>
                            <p><strong>Total :</strong> {commande.Total.ToString("N0")} FCFA</p>
                            <p><strong>Statut :</strong> ⏳ En attente</p>
                        </div>
                        <p>Merci de votre confiance !</p>
                    </div>
                    <div style='background:#1a1a2e; padding:20px; text-align:center;'>
                        <p style='color:#888;'>© 2026 ModaSen — Dakar, Sénégal</p>
                    </div>
                </div>";

                GMailer.SendMail(client.Email, sujet, body);
            }
            catch { /* Si email échoue, la commande continue */ }

            return RedirectToAction("Confirmation", new { id = commande.ID });
        }

        // Page confirmation
        public async Task<IActionResult> Confirmation(int id)
        {
            var commande = await _db.Commandes
                .Include(c => c.LignesCommande)
                .ThenInclude(l => l.Produit)
                .Include(c => c.Client)
                .FirstOrDefaultAsync(c => c.ID == id);

            return View(commande);
        }

        // Mes commandes
        public async Task<IActionResult> MesCommandes()
        {
            var clientID = HttpContext.Session.GetInt32("ClientID");
            if (clientID == null) return RedirectToAction("Login", "Auth");

            var commandes = await _db.Commandes
                .Include(c => c.LignesCommande)
                .ThenInclude(l => l.Produit)
                .Where(c => c.ClientID == clientID)
                .OrderByDescending(c => c.DateCommande)
                .ToListAsync();

            return View(commandes);
        }
        // Télécharger facture PDF
        public async Task<IActionResult> Facture(int id)
        {
            var commande = await _db.Commandes
                .Include(c => c.Client)
                .Include(c => c.LignesCommande)
                .ThenInclude(l => l.Produit)
                .FirstOrDefaultAsync(c => c.ID == id);

            if (commande == null) return NotFound();

            var pdfService = new PdfService();
            var pdfBytes = pdfService.GenererFacture(commande);

            return File(pdfBytes, "application/pdf", $"Facture_ModaSen_{id}.pdf");
        }
    }
}