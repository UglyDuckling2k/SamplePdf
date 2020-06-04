using SamplePdf.Models.Pdf;
using SamplePdf.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SamplePdf.Controllers
{
    public class PdfController : Controller
    {
        private readonly IPdfService _pdfService;

        public PdfController(IPdfService pdfService)
        {
            _pdfService = pdfService;
        }

        public FileStreamResult UrlAsPdf()
        {
            var filename = "UrlAsPdf.pdf";
            var url = new Uri(Url.Action("Sample", "Pdf"), UriKind.Relative);

            var result = _pdfService.GenerateUrlToPdf(url, filename, null);

            return result;
        }

        public FileStreamResult HtmlAsPdf()
        {
            var model = new SampleModel() { Text = "Hello HtmlAsPdf!" };

            var filename = "HtmlAsPdf.pdf";
            var html = this.RenderViewToString("~/Views/Pdf/Sample.cshtml", model);

            var result = _pdfService.GenerateHtmlToPdf(html, filename, null);

            return result;
        }

        public ActionResult Sample()
        {
            var model = new SampleModel() { Text = "Hello UrlAsPdf!" };
            return View(model);
        }
    }
}