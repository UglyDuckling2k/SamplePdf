using IronPdf;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;

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


        FileStreamResult RenderUrlToPdf(Uri url, string filename, PdfSettings pdfSettings = null);

        FileStreamResult RenderHtmlToPdf(string html, string filename, PdfSettings pdfSettings = null);
    }

    public class PdfService : IPdfService
    {


        private static int index = 0;
        private static object Lock = new object();

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

        public FileStreamResult RenderUrlToPdf(Uri url, string filename, PdfSettings pdfSettings = null)
        {
            var absoluteUrl = url.ToAbsolute();

            var renderer = CreateRenderer(filename, pdfSettings);
            var pdf = renderer.RenderUrlAsPdf(absoluteUrl);

            var returnStream = new FileStreamResult(pdf.Stream, "application/pdf");
            returnStream.FileDownloadName = filename;

            return returnStream;
        }

        public FileStreamResult RenderHtmlToPdf(string html, string filename, PdfSettings pdfSettings = null)
        {
            var renderer = CreateRenderer(filename, pdfSettings);
            var pdf = renderer.RenderHtmlAsPdf(html);

            var returnStream = new FileStreamResult(pdf.Stream, "application/pdf");
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
