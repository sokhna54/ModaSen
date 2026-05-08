using AppVetements.Models;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Pdf.Canvas.Draw;

namespace AppVetements.Helpers
{
    public class PdfService
    {
        public byte[] GenererFacture(Commande commande)
        {
            using var ms = new MemoryStream();
            var writer = new PdfWriter(ms);
            var pdf = new PdfDocument(writer);
            var doc = new Document(pdf);

            // Fonts
            var fontBold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            var fontNormal = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

            // Couleurs
            var couleurPrimaire = new DeviceRgb(26, 26, 46);
            var couleurOr = new DeviceRgb(240, 165, 0);
            var couleurGris = new DeviceRgb(248, 248, 248);

            // ===== EN-TÊTE =====
            var header = new Table(2).UseAllAvailableWidth();

            var cellLogo = new Cell()
                .SetBorder(iText.Layout.Borders.Border.NO_BORDER)
                .Add(new Paragraph("ModaSen")
                    .SetFontSize(24)
                    .SetFont(fontBold)
                    .SetFontColor(couleurPrimaire))
                .Add(new Paragraph("Dakar, Senegal")
                    .SetFontSize(10)
                    .SetFont(fontNormal)
                    .SetFontColor(ColorConstants.GRAY));
            header.AddCell(cellLogo);

            var cellTitre = new Cell()
                .SetBorder(iText.Layout.Borders.Border.NO_BORDER)
                .SetTextAlignment(TextAlignment.RIGHT)
                .Add(new Paragraph("FACTURE")
                    .SetFontSize(28)
                    .SetFont(fontBold)
                    .SetFontColor(couleurOr))
                .Add(new Paragraph($"N° {commande.ID:D6}")
                    .SetFontSize(12)
                    .SetFont(fontNormal)
                    .SetFontColor(ColorConstants.GRAY))
                .Add(new Paragraph($"Date : {commande.DateCommande:dd/MM/yyyy}")
                    .SetFontSize(10)
                    .SetFont(fontNormal)
                    .SetFontColor(ColorConstants.GRAY));
            header.AddCell(cellTitre);

            doc.Add(header);
            doc.Add(new LineSeparator(new SolidLine())
                .SetMarginTop(10)
                .SetMarginBottom(20));

            // ===== INFO CLIENT =====
            var infoTable = new Table(2).UseAllAvailableWidth().SetMarginBottom(20);

            var cellClient = new Cell()
                .SetBorder(iText.Layout.Borders.Border.NO_BORDER)
                .SetBackgroundColor(couleurGris)
                .SetPadding(15)
                .Add(new Paragraph("FACTURE A")
                    .SetFont(fontBold)
                    .SetFontColor(couleurPrimaire)
                    .SetFontSize(10))
                .Add(new Paragraph(commande.Client?.NomComplet ?? "")
                    .SetFontSize(12)
                    .SetFont(fontBold))
                .Add(new Paragraph(commande.Client?.Email ?? "")
                    .SetFontSize(10)
                    .SetFont(fontNormal)
                    .SetFontColor(ColorConstants.GRAY))
                .Add(new Paragraph(commande.Client?.Telephone ?? "")
                    .SetFontSize(10)
                    .SetFont(fontNormal)
                    .SetFontColor(ColorConstants.GRAY));
            infoTable.AddCell(cellClient);

            var cellStatut = new Cell()
                .SetBorder(iText.Layout.Borders.Border.NO_BORDER)
                .SetBackgroundColor(couleurGris)
                .SetPadding(15)
                .SetTextAlignment(TextAlignment.RIGHT)
                .Add(new Paragraph("STATUT")
                    .SetFont(fontBold)
                    .SetFontColor(couleurPrimaire)
                    .SetFontSize(10))
                .Add(new Paragraph(commande.Statut)
                    .SetFontSize(12)
                    .SetFont(fontBold))
                .Add(new Paragraph($"Commande #{commande.ID}")
                    .SetFontSize(10)
                    .SetFont(fontNormal)
                    .SetFontColor(ColorConstants.GRAY));
            infoTable.AddCell(cellStatut);

            doc.Add(infoTable);

            // ===== TABLEAU DES ARTICLES =====
            var tableau = new Table(new float[] { 3, 1, 1, 1 }).UseAllAvailableWidth();

            foreach (var titre in new[] { "ARTICLE", "PRIX UNITAIRE", "QTE", "TOTAL" })
            {
                tableau.AddHeaderCell(new Cell()
                    .SetBackgroundColor(couleurPrimaire)
                    .SetFontColor(ColorConstants.WHITE)
                    .SetPadding(10)
                    .SetFont(fontBold)
                    .Add(new Paragraph(titre).SetFontSize(10)));
            }

            bool pair = false;
            foreach (var ligne in commande.LignesCommande)
            {
                var bg = pair ? couleurGris : ColorConstants.WHITE;

                tableau.AddCell(new Cell()
                    .SetBackgroundColor(bg)
                    .SetPadding(10)
                    .Add(new Paragraph(ligne.Produit?.NomProduit ?? "")
                        .SetFont(fontNormal)
                        .SetFontSize(10)));

                tableau.AddCell(new Cell()
                    .SetBackgroundColor(bg)
                    .SetPadding(10)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .Add(new Paragraph($"{ligne.PrixUnitaire:N0} FCFA")
                        .SetFont(fontNormal)
                        .SetFontSize(10)));

                tableau.AddCell(new Cell()
                    .SetBackgroundColor(bg)
                    .SetPadding(10)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .Add(new Paragraph($"{ligne.Quantite}")
                        .SetFont(fontNormal)
                        .SetFontSize(10)));

                tableau.AddCell(new Cell()
                    .SetBackgroundColor(bg)
                    .SetPadding(10)
                    .SetTextAlignment(TextAlignment.RIGHT)
                    .Add(new Paragraph($"{(ligne.PrixUnitaire * ligne.Quantite):N0} FCFA")
                        .SetFont(fontBold)
                        .SetFontSize(10)));

                pair = !pair;
            }

            doc.Add(tableau);

            // ===== TOTAL =====
            var totalTable = new Table(2).UseAllAvailableWidth().SetMarginTop(10);

            totalTable.AddCell(new Cell()
                .SetBorder(iText.Layout.Borders.Border.NO_BORDER));

            totalTable.AddCell(new Cell()
                .SetBorder(iText.Layout.Borders.Border.NO_BORDER)
                .SetBackgroundColor(couleurPrimaire)
                .SetPadding(15)
                .SetTextAlignment(TextAlignment.RIGHT)
                .Add(new Paragraph($"TOTAL : {commande.Total:N0} FCFA")
                    .SetFontSize(16)
                    .SetFont(fontBold)
                    .SetFontColor(couleurOr)));

            doc.Add(totalTable);

            // ===== PIED DE PAGE =====
            doc.Add(new Paragraph("\n"));
            doc.Add(new LineSeparator(new SolidLine()));
            doc.Add(new Paragraph("Merci pour votre confiance ! - ModaSen 2026")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(9)
                .SetFont(fontNormal)
                .SetFontColor(ColorConstants.GRAY)
                .SetMarginTop(10));

            doc.Close();
            return ms.ToArray();
        }
    }
}