using System.ComponentModel.DataAnnotations;

namespace AppVetements.Models
{
    public class Produit
    {
        [Key]
        public int ID { get; set; }

        [Required, MaxLength(150)]
        public string NomProduit { get; set; }

        [Required]
        public decimal Prix { get; set; }

        public int Stock { get; set; }

        public string ImageProduit { get; set; }

        public string Description { get; set; }

        // ✅ Nouveau champ promo
        public int Remise { get; set; } = 0; // En pourcentage ex: 20 = 20%

        // ✅ Prix après remise (calculé automatiquement)
        public decimal PrixPromo => Remise > 0 ? Prix - (Prix * Remise / 100) : Prix;

        public bool EnPromo => Remise > 0;

        public int CategorieID { get; set; }
        public Categorie Categorie { get; set; }
    }
}