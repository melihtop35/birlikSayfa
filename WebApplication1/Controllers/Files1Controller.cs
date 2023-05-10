using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Metrics;
using WebApplication1.Models;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace WebApplication1.Controllers
{
	public class Files1Controller : Controller
	{
		IHostingEnvironment _hostingEnvironment = null;
		public Files1Controller(IHostingEnvironment hostingEnvironment)
		{
			_hostingEnvironment = hostingEnvironment;
		}
		[HttpGet]
		public IActionResult Index(string fileName = "")
		{
			string Session = HttpContext.Session.GetString("VergiNo");

			FileClass fileobj = new FileClass();
			fileobj.Name = fileName;
			string path = $"{_hostingEnvironment.WebRootPath}\\files\\";
			int nId = 1;
			foreach (string pdfPath in Directory.EnumerateFiles(path, "*.pdf"))
			{
				if (Session!=null && pdfPath.Contains(Session) && pdfPath.Contains("İmza Sirküleri"))

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
		public IActionResult Index(IFormFile file, [FromServices] IHostingEnvironment hostingEnvironment)
		{
			var Session = HttpContext.Session.GetString("VergiNo");
			try
			{
				string fileName = $"{hostingEnvironment.WebRootPath}\\files\\{Session}_İmza Sirküleri_{file.FileName}";
				using (FileStream fileStream = System.IO.File.Create(fileName))
				{
					file.CopyTo(fileStream);
					fileStream.Flush();
				}
				return Index();
			}
			catch (NullReferenceException)
			{
				return Redirect("/Files1/Index");
			}
		}
		public IActionResult PDFViewerNewTab(string fileName)
		{
			string path = _hostingEnvironment.WebRootPath + "\\files\\" + fileName;
			return File(System.IO.File.ReadAllBytes(path), "application/pdf");
		}
		public IActionResult Yonlendirme()
		{
			return View();
		}
	}
}
