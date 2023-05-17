using Microsoft.AspNetCore.Mvc;
using QRCoder;
using System.Drawing.Imaging;
using System.Drawing;
using sayfaASP.Models;
using WebApplication1.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace WebApplication1.Controllers
{
    public class QRCodeController : Controller
    {
        private readonly DataContext _context;

        public QRCodeController(DataContext context)
        {
            _context = context;
        }

        public IActionResult QRCode()
        {
            var sessionValue = HttpContext.Session.GetString("VergiNo");

            var users = (from u in _context.Users
                         join ui in _context.UsersInfo on u.Name equals ui.NameId
                         join uu in _context.UsersUnit on u.Unit equals uu.UnitId
                         where u.taxNo == sessionValue
                         select new UserViewModel
                         {
                             Id = u.Id,
                             taxNo = u.taxNo,
                             tcNo = u.tcNo,
                             Email = u.Email,
                             UnitAd = uu.UnitAd,
                             NameUser = ui.NameUser,
                             Surname = ui.Surname,
                             Phone = ui.Phone
                         }).ToList();

            ViewBag.UserList = users;

            // QR kodu oluşturma
            var qrCodeText = "https://www.youtube.com/watch?v=dQw4w9WgXcQ&pp=ygUXbmV2ZXIgZ29ubmEgZ2l2ZSB5b3UgdXA%3D";
            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(qrCodeText, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new QRCode(qrCodeData);
            var qrCodeImage = qrCode.GetGraphic(5);

            using (var stream = new MemoryStream())
            {
                qrCodeImage.Save(stream, ImageFormat.Png);
                var qrCodeBase64 = Convert.ToBase64String(stream.ToArray());
                ViewBag.QRCodeBase64 = qrCodeBase64;
            }

            return View();
        }
    }
}
