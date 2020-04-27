using IronPdf;
using System;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace SamplePdf.Services
{
    public class PdfSettings
    {
        public string Title { get; set; }

        public bool IsPreview { get; set; }

        public PdfSettings()
        {
            IsPreview = false;
            Title = null;
        }
    }

    public interface IPdfService
    {
        void Initialize();

        /// <summary>
        /// Generates a PDF stream that is sent back to user
        /// </summary>
        /// <param name="url">Source Url for PDF contents</param>
        /// <returns>MemoryStream</returns>
        MemoryStream GeneratePdf(Uri url, PdfSettings pdfSettings = null);

        /// <summary>
        /// Generates a PDF documents that is sent back to user
        /// </summary>
        /// <param name="filename">Name of returned file</param>
        /// <param name="url">source Url for PDF contents</param>
        /// <returns>PDF FileStreamResult</returns>
        FileStreamResult GeneratePdf(Uri url, string filename, PdfSettings pdfSettings = null);
    }

    public class PdfService : IPdfService
    {
        public PdfService()
        {

        }

        public void Initialize()
        {
            Installation.TempFolderPath = ConfigurationManager
                .AppSettings["IronPdf.TempPath"];
            //License.LicenseKey = ConfigurationManager
            //    .AppSettings["IronPdf.LicenseKey"];
        }

        public MemoryStream GeneratePdf(Uri url, PdfSettings pdfSettings = null)
        {
            var absoluteUrl = url.ToAbsolute();

            var pdfRenderer = CreateRenderer(null, pdfSettings);
            var pdf = pdfRenderer.RenderUrlAsPdf(absoluteUrl);

            return pdf.Stream;
        }

        public FileStreamResult GeneratePdf(Uri url, string filename, PdfSettings pdfSettings = null)
        {
            var absoluteUrl = url.ToAbsolute();

            var pdfRenderer = CreateRenderer(filename, pdfSettings);
            var pdf = pdfRenderer.RenderUrlAsPdf(absoluteUrl);

            FileStreamResult returnStream = new FileStreamResult(pdf.Stream, "application/pdf");
            returnStream.FileDownloadName = filename;

            return returnStream;
        }

        private HtmlToPdf CreateRenderer(string filename = null, PdfSettings pdfSettings = null)
        {
            if (pdfSettings == null)
                pdfSettings = new PdfSettings();

            var header = "<div style=\"overflow: auto;\">";

            if (pdfSettings.IsPreview)
            {
                header += "<span style=\"display:inline-block; float: left; font-size: 20px; color: #909090\">Förhandsgranskning</span>";
            }

            header += "<span style=\"display:inline-block; float:right; color: #909090\">{page} / {total-pages}</span>";
            header += "</div>";

            var title = pdfSettings.Title;
            if (string.IsNullOrEmpty(title))
            {
                title = filename;
            }

            var renderer = new IronPdf.HtmlToPdf();
            renderer.PrintOptions.PaperSize = PdfPrintOptions.PdfPaperSize.A4;
            renderer.PrintOptions.ViewPortWidth = 1024;
            renderer.PrintOptions.Title = title;
            renderer.PrintOptions.EnableJavaScript = true;
            renderer.PrintOptions.RenderDelay = 50; //ms
            renderer.PrintOptions.CssMediaType = PdfPrintOptions.PdfCssMediaType.Screen;
            renderer.PrintOptions.MarginTop = 25;
            renderer.PrintOptions.MarginBottom = 10;
            renderer.PrintOptions.MarginLeft = 25;
            renderer.PrintOptions.MarginRight = 25;
            renderer.PrintOptions.Header = new HtmlHeaderFooter()
            {
                Height = 15,
                Spacing = 5,
                HtmlFragment = header
            };

            var authCookieName = ConfigurationManager.AppSettings["Auth.OwinCookie"];
            var authCookie = HttpContext.Current.Request.Cookies.Get(authCookieName);
            if (authCookie != null)
            {
                renderer.LoginCredentials.EnableCookies = true;
                renderer.LoginCredentials.CustomCookies[authCookie.Name] = authCookie.Value;
            }

            return renderer;
        }
    }
}
