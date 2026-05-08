using System.ComponentModel.DataAnnotations;

namespace AppVetements.Models
{
    public class Commande
    {
        [Key]
        public int ID { get; set; }

        public DateTime DateCommande { get; set; } = DateTime.Now;

        public decimal Total { get; set; }

        public string Statut { get; set; } = "En attente";

        // Clé étrangère
        public int ClientID { get; set; }
        public Client Client { get; set; }

        // Navigation
        public List<LigneCommande> LignesCommande { get; set; }
    }
}