using Microsoft.AspNetCore.Mvc;
using sayfaASP.Models;
using System.Diagnostics;
using WebApplication1.Filter;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [UserFilter]
    public class HomeController : Controller
    {
        private readonly DataContext _context;
        public HomeController(DataContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult List()
        {
            var users = from u in _context.Users
                        join ui in _context.UsersInfo on u.Name equals ui.NameId
                        join uu in _context.UsersUnit on u.Unit equals uu.UnitId
                        select new
                        {
                            u.Id,
                            u.taxNo,
                            u.tcNo,
                            u.Email,
                            uu.UnitAd,
                            ui.NameUser,
                            ui.Surname,
                            ui.Phone
                        };

            var viewModel = users.Select(u => new UserViewModel
            {
                Id = u.Id,
                taxNo = u.taxNo,
                tcNo = u.tcNo,
                Email = u.Email,
                UnitAd = u.UnitAd,
                NameUser = u.NameUser,
                Surname = u.Surname,
                Phone = u.Phone
            }).ToList();

            var Session = HttpContext.Session.GetString("VergiNo");
            var SessionTc = HttpContext.Session.GetString("TcNo");
            return View(viewModel.Where(x => x.taxNo.Equals(Session) || x.tcNo.Equals(SessionTc)));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Secim()
        {
            return View();
        }

        public IActionResult Index2()
        {
            List<FileClass> files = new List<FileClass>();

            // files klasöründeki dosyaları yükle
            foreach (string file in Directory.GetFiles("C:\\Users\\melih_o\\Downloads\\sayfaASP-master\\sayfaASP-master\\WebApplication1\\wwwroot\\İmza Sirküleri\\"))
            {
                FileClass fileItem = new FileClass();
                fileItem.Name = Path.GetFileName(file);
                fileItem.Path = file;
                fileItem.FolderName = "İmza Sirküleri"; // Klasör adını ayarlayın
                files.Add(fileItem);
            }

            // files2 klasöründeki dosyaları yükle
            foreach (string file in Directory.GetFiles("C:\\Users\\melih_o\\Downloads\\sayfaASP-master\\sayfaASP-master\\WebApplication1\\wwwroot\\Vergi Levhası\\"))
            {
                FileClass fileItem = new FileClass();
                fileItem.Name = Path.GetFileName(file);
                fileItem.Path = file;
                fileItem.FolderName = "Vergi Levhası"; // Klasör adını ayarlayın
                files.Add(fileItem);
            }

            // files3 klasöründeki dosyaları yükle
            foreach (string file in Directory.GetFiles("C:\\Users\\melih_o\\Downloads\\sayfaASP-master\\sayfaASP-master\\WebApplication1\\wwwroot\\files3\\"))
            {
                FileClass fileItem = new FileClass();
                fileItem.Name = Path.GetFileName(file);
                fileItem.Path = file;
                fileItem.FolderName = "files3"; // Klasör adını ayarlayın
                files.Add(fileItem);
            }

            return View(files);
        }



        [HttpPost]
        public IActionResult ProcessSelection(Dictionary<string, string> selectedFiles)
        {
            // Yeni bir klasör oluşturma
            string newFolderPath = Path.Combine("C:\\Users\\melih_o\\Downloads\\sayfaASP-master\\sayfaASP-master\\WebApplication1\\wwwroot\\", "NewFolder");
            Directory.CreateDirectory(newFolderPath);

            foreach (string existingFile in Directory.GetFiles(newFolderPath))
            {
                System.IO.File.Delete(existingFile);
            }

            // Seçilen dosyaları işleme
            foreach (var folderName in selectedFiles.Keys)
            {
                var selectedFile = selectedFiles[folderName];

                // Dosyayı yeni klasöre taşıma
                string fileName = Path.GetFileName(selectedFile);
                string newFilePath = Path.Combine(newFolderPath, fileName);
                if (System.IO.File.Exists(newFilePath))
                {
                    System.IO.File.Delete(newFilePath);
                }
                System.IO.File.Copy(selectedFile, newFilePath);
            }

            return View("Index3");
        }
        public IActionResult Index3()
        {
            return View();
        }

    }
}