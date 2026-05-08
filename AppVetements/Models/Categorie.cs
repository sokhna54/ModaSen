using System.ComponentModel.DataAnnotations;

namespace AppVetements.Models
{
    public class Categorie
    {
        [Key]
        public int ID { get; set; }

        [Required, MaxLength(100)]
        public string NomCategorie { get; set; }

        // Navigation
        public List<Produit> Produits { get; set; }
    }
}