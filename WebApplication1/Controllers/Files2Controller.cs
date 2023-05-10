using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Metrics;
using WebApplication1.Models;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace WebApplication1.Controllers
{
    public class Files2Controller : Controller
    {
        IHostingEnvironment _hostingEnvironment = null;
        public Files2Controller(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        [HttpGet]
        public IActionResult Index2(string fileName = "")
        {
            string Session = HttpContext.Session.GetString("VergiNo");

            FileClass fileobj = new FileClass();
            fileobj.Name = fileName;
            string path = $"{_hostingEnvironment.WebRootPath}\\files2\\";
            int nId = 1;
            foreach (string pdfPath in Directory.EnumerateFiles(path, "*.pdf"))
            {
                if (Session != null && pdfPath.Contains(Session) && pdfPath.Contains("Vergi Levhası"))
                {
                    string vs = Session;

                    if (Session == vs)
                    {
                        fileobj.Files.Add(new FileClass()
                        {
                            FileId = nId++,
                            Name = Path.GetFileName(pdfPath),
                            Path = pdfPath
                        });
                    }
                }
                else
                {
                    return Redirect("/Files1/Yonlendirme");
                }
            }
            return View(fileobj);
        }
        [HttpPost]
        public IActionResult Index2(IFormFile file, [FromServices] IHostingEnvironment hostingEnvironment)
        {
            var Session = HttpContext.Session.GetString("VergiNo");
            try
            {
                string fileName = $"{hostingEnvironment.WebRootPath}\\files2\\{Session}_Vergi Levhası_{file.FileName}";
                using (FileStream fileStream = System.IO.File.Create(fileName))
                {
                    file.CopyTo(fileStream);
                    fileStream.Flush();
                }
                return Index2();
            }
            catch (NullReferenceException)
            {
                return Redirect("/Files1/Index2");
            }
        }
        public IActionResult PDFViewerNewTab(string fileName)
        {
            string path = _hostingEnvironment.WebRootPath + "\\files2\\" + fileName;
            return File(System.IO.File.ReadAllBytes(path), "application/pdf");
        }
    }
}
