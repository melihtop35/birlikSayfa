using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Metrics;
using WebApplication1.Models;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace WebApplication1.Controllers
{
    public class Files3Controller : Controller
    {
        IHostingEnvironment _hostingEnvironment = null;
        public Files3Controller(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        [HttpGet]
        public IActionResult Index3(string fileName = "")
        {
            string Session = HttpContext.Session.GetString("VergiNo");

            FileClass fileobj = new FileClass();
            fileobj.Name = fileName;
            string path = $"{_hostingEnvironment.WebRootPath}\\files3\\";
            int nId = 1;
            foreach (string pdfPath in Directory.EnumerateFiles(path, "*.pdf"))
            {
                if (Session != null && pdfPath.Contains(Session) && pdfPath.Contains("Ney"))
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
        public IActionResult Index3(IFormFile file, [FromServices] IHostingEnvironment hostingEnvironment)
        {
            var Session = HttpContext.Session.GetString("VergiNo");
            try
            {
                string fileName = $"{hostingEnvironment.WebRootPath}\\files3\\{Session}_Ney_{file.FileName}";
                using (FileStream fileStream = System.IO.File.Create(fileName))
                {
                    file.CopyTo(fileStream);
                    fileStream.Flush();
                }
                return Index3();
            }
            catch (NullReferenceException)
            {
                return Redirect("/Files1/Index");
            }
        }
        public IActionResult PDFViewerNewTab(string fileName)
        {
            string path = _hostingEnvironment.WebRootPath + "\\files3\\" + fileName;
            return File(System.IO.File.ReadAllBytes(path), "application/pdf");
        }
    }
}
