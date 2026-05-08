using System.ComponentModel.DataAnnotations;

namespace AppVetements.Models
{
    public class Client
    {
        [Key]
        public int ID { get; set; }

        [Required, MaxLength(100)]
        public string NomComplet { get; set; }

        [Required, MaxLength(80)]
        public string Email { get; set; }

        [MaxLength(20)]
        public string Telephone { get; set; }

        public string Adresse { get; set; }

        [Required]
        public string MotDePasse { get; set; }

        // Navigation
        public List<Commande> Commandes { get; set; }
    }
}