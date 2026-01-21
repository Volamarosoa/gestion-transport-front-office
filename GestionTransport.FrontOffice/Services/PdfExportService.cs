using IronPdf;

namespace GestionTransport.FrontOffice.Services
{
    public class PdfExportService
    {
        public PdfExportService()
        {
            // Configuration de la licence (pour développement, c'est gratuit)
            // IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY"; // Pas nécessaire en développement
        }

        public byte[] GeneratePdfFromHtml(string htmlContent)
        {
            var renderer = new ChromePdfRenderer();
            
            // Configuration du rendu
            renderer.RenderingOptions.PaperOrientation = IronPdf.Rendering.PdfPaperOrientation.Landscape;
            renderer.RenderingOptions.PaperSize = IronPdf.Rendering.PdfPaperSize.A4;
            renderer.RenderingOptions.MarginTop = 20;
            renderer.RenderingOptions.MarginBottom = 20;
            renderer.RenderingOptions.MarginLeft = 20;
            renderer.RenderingOptions.MarginRight = 20;
            renderer.RenderingOptions.CssMediaType = IronPdf.Rendering.PdfCssMediaType.Print;
            
            // Générer le PDF
            var pdf = renderer.RenderHtmlAsPdf(htmlContent);
            
            return pdf.BinaryData;
        }

        public byte[] GeneratePdfFromUrl(string url)
        {
            var renderer = new ChromePdfRenderer();
            
            renderer.RenderingOptions.PaperOrientation = IronPdf.Rendering.PdfPaperOrientation.Landscape;
            renderer.RenderingOptions.PaperSize = IronPdf.Rendering.PdfPaperSize.A4;
            
            var pdf = renderer.RenderUrlAsPdf(url);
            
            return pdf.BinaryData;
        }
    }
}