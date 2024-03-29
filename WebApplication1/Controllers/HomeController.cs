﻿using Microsoft.AspNetCore.Mvc;
using sayfaASP.Models;
using System.Diagnostics;
using WebApplication1.Filter;
using WebApplication1.Models;
using QRCoder;
using System.Drawing.Imaging;
using System.Drawing;

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
        public IActionResult Yonlendirme()
        {
            return View();
        }
        public IActionResult Yonelme()
        {
            return View();
        }
        public IActionResult Index()
        {
            var Session = HttpContext.Session.GetString("VergiNo");
            var SessionTc = HttpContext.Session.GetString("TcNo");
            var users = from u in _context.Users
                        join ui in _context.UsersInfo on u.Name equals ui.NameId
                        join uu in _context.UsersUnit on u.Unit equals uu.UnitId
                        join uo in _context.UsersCom on u.Com equals uo.ComId
                        select new
                        {
                            u.Id,
                            uo.ComAd,
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
                ComAd = u.ComAd,
                taxNo = u.taxNo,
                tcNo = u.tcNo,
                Email = u.Email,
                UnitAd = u.UnitAd,
                NameUser = u.NameUser,
                Surname = u.Surname,
                Phone = u.Phone
            }).ToList();

            ViewData["ComAd"] = viewModel.Where(u => u.taxNo == Session).Select(u => u.ComAd).FirstOrDefault();   // ComAd değerini ViewData üzerinden aktarın

            return View();
        }
        public IActionResult List()
        {
            var users = from u in _context.Users
                        join ui in _context.UsersInfo on u.Name equals ui.NameId
                        join uu in _context.UsersUnit on u.Unit equals uu.UnitId
                        join uo in _context.UsersCom on u.Com equals uo.ComId
                        select new
                        {
                            u.Id,
                            uo.ComAd,
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
                ComAd = u.ComAd,
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
            var sessionValue = HttpContext.Session.GetString("VergiNo");
            if (sessionValue != null)
            {
                // members klasöründe session VergiNo değerinden alınan ada sahip bir klasör var mı kontrol et
                string membersFolderPath = "C:\\Users\\melih_o\\Downloads\\sayfaASP-master\\sayfaASP-master\\WebApplication1\\wwwroot\\Members\\";
                string sessionFolderPath = Path.Combine(membersFolderPath, sessionValue);
                bool sessionFolderExists = Directory.Exists(sessionFolderPath);
                if (sessionFolderExists)
                {
                    return Redirect("/Home/Yonlendirme");
                }

                string signedFolderPath = "C:\\Users\\melih_o\\Downloads\\sayfaASP-master\\sayfaASP-master\\WebApplication1\\wwwroot\\Members\\SignedPDFs\\";
                string FolderPath = Path.Combine(signedFolderPath, sessionValue);
                bool FolderExists = Directory.Exists(FolderPath);
                if (FolderExists)
                {
                    return Redirect("/Home/Yonelme");
                }
            }
            return View();
        }

        public IActionResult Index2()
        {
            var sessionValue = HttpContext.Session.GetString("VergiNo");
            if (sessionValue != null)
            {
                // members klasöründe session VergiNo değerinden alınan ada sahip bir klasör var mı kontrol et
                string membersFolderPath = "C:\\Users\\melih_o\\Downloads\\sayfaASP-master\\sayfaASP-master\\WebApplication1\\wwwroot\\Members\\";
                string sessionFolderPath = Path.Combine(membersFolderPath, sessionValue);
                bool sessionFolderExists = Directory.Exists(sessionFolderPath);

                if (sessionValue == null)
                {
                    return Redirect("/Files1/Yonlendirme");
                }
                if (sessionFolderExists)
                {
                    return Redirect("/Home/Yonlendirme");
                }
                string signedFolderPath = "C:\\Users\\melih_o\\Downloads\\sayfaASP-master\\sayfaASP-master\\WebApplication1\\wwwroot\\Members\\SignedPDFs\\";
                string FolderPath = Path.Combine(signedFolderPath, sessionValue);
                bool FolderExists = Directory.Exists(FolderPath);
                if (FolderExists)
                {
                    return Redirect("/Home/Yonelme");
                }

                List<FileClass> files = new List<FileClass>();

                // İmza Sirküleri klasöründeki dosyaları yükle
                foreach (string file in Directory.GetFiles("wwwroot\\İmza Sirküleri\\"))
                {
                    if (Path.GetFileName(file).Contains(sessionValue))
                    {
                        FileClass fileItem = new FileClass();
                        fileItem.Name = Path.GetFileName(file);
                        fileItem.Path = file;
                        fileItem.FolderName = "İmza Sirküleri"; // Klasör adını ayarlayın
                        files.Add(fileItem);
                    }
                }

                // Vergi Levhası klasöründeki dosyaları yükle
                foreach (string file in Directory.GetFiles("wwwroot\\Vergi Levhası\\"))
                {
                    if (Path.GetFileName(file).Contains(sessionValue))
                    {
                        FileClass fileItem = new FileClass();
                        fileItem.Name = Path.GetFileName(file);
                        fileItem.Path = file;
                        fileItem.FolderName = "Vergi Levhası"; // Klasör adını ayarlayın
                        files.Add(fileItem);
                    }
                }

                // files3 klasöründeki dosyaları yükle
                foreach (string file in Directory.GetFiles("wwwroot\\files3\\"))
                {
                    if (Path.GetFileName(file).Contains(sessionValue))
                    {
                        FileClass fileItem = new FileClass();
                        fileItem.Name = Path.GetFileName(file);
                        fileItem.Path = file;
                        fileItem.FolderName = "files3"; // Klasör adını ayarlayın
                        files.Add(fileItem);
                    }
                }

                return View(files);
            }
            return Redirect("/Files1/Yonlendirme");
        }





        [HttpPost]
        public IActionResult ProcessSelection(Dictionary<string, string> selectedFiles)
        {
            var sessionValue = HttpContext.Session.GetString("VergiNo");
            // Yeni bir klasör oluşturma
            string newFolderPath = Path.Combine("wwwroot\\Members", sessionValue);
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