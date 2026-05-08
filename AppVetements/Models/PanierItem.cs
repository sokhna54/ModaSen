namespace AppVetements.Models
{
    public class PanierItem
    {
        public int ProduitID { get; set; }
        public string NomProduit { get; set; }
        public decimal Prix { get; set; }
        public int Quantite { get; set; }
        public string ImageProduit { get; set; }
        public decimal Total => Prix * Quantite;
    }
}