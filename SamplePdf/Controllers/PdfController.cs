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

        public FileStreamResult SampleAsPdf()
        {
            var filename = "sample.pdf";
            var uri = new Uri(Url.Action("Sample", "Pdf"), UriKind.Relative);

            var result = _pdfService.GeneratePdf(uri, filename, null);

            return result;
        }

        public ActionResult Sample()
        {
            return View();
        }
    }
}