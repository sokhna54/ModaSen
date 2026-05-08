using System.ComponentModel.DataAnnotations;

namespace AppVetements.Models
{
    public class LigneCommande
    {
        [Key]
        public int ID { get; set; }

        public int Quantite { get; set; }

        public decimal PrixUnitaire { get; set; }

        // Clés étrangères
        public int CommandeID { get; set; }
        public Commande Commande { get; set; }

        public int ProduitID { get; set; }
        public Produit Produit { get; set; }
    }
}