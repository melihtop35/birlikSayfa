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
		public IActionResult Index(string fileName = "", bool isDelete=false)
		{
			string Session = HttpContext.Session.GetString("VergiNo");

			FileClass fileobj = new FileClass();
			fileobj.Name = fileName;
			string path = $"{_hostingEnvironment.WebRootPath}\\İmza Sirküleri\\";
			int nId = 1;

			if (isDelete)
			{
				string filePath = $"{_hostingEnvironment.WebRootPath}\\İmza Sirküleri\\{fileobj.Name}";
				if (System.IO.File.Exists(filePath))
				{
					System.IO.File.Delete(filePath);
				}
			}

            if (Session != null)
            {
				foreach (string pdfPath in Directory.EnumerateFiles(path, "*.pdf"))
				{
					if (pdfPath.Contains(Session))
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
				}
			}
			else if(Session==null)
			{
				return Redirect("/Files1/Yonlendirme");
			}
			
			return View(fileobj);
		}
		[HttpPost]
		public IActionResult Index(IFormFile file, [FromServices] IHostingEnvironment hostingEnvironment)
		{
			var Session = HttpContext.Session.GetString("VergiNo");
			try
			{
				string fileName = $"{hostingEnvironment.WebRootPath}\\İmza Sirküleri\\{Session}_İmza Sirküleri_{file.FileName}";
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
			string path = _hostingEnvironment.WebRootPath + "\\İmza Sirküleri\\" + fileName;
			return File(System.IO.File.ReadAllBytes(path), "application/pdf");
		}
		public IActionResult Yonlendirme()
		{
			return View();
		}
	}
}
