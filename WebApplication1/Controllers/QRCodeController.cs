using Microsoft.AspNetCore.Mvc;
using QRCoder;
using System.Drawing.Imaging;
using System.Drawing;
using sayfaASP.Models;
using WebApplication1.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace WebApplication1.Controllers
{
    public class QRCodeController : Controller
    {
        private readonly DataContext _context;

        public QRCodeController(DataContext context)
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
        public IActionResult QRCode()
        {
            var sessionValue = HttpContext.Session.GetString("VergiNo");
            var sessionTcNo = HttpContext.Session.GetString("TcNo");

            if (string.IsNullOrEmpty(sessionValue))
            {
                // sessionValue boş ise veritabanından tcNo'nun eşit olduğu vergiNo'yu al
                sessionValue = _context.Users.FirstOrDefault(u => u.tcNo == sessionTcNo)?.taxNo;
            }

            if (string.IsNullOrEmpty(sessionValue))
            {
                // sessionValue hala boş ise yönlendirme yapma
                return Redirect("/QRCode/Yonelme");
            }

            // members klasöründe sessionValue değerine sahip bir klasör var mı kontrol et
            string signedFolderPath = "C:\\Users\\melih_o\\Downloads\\sayfaASP-master\\sayfaASP-master\\WebApplication1\\wwwroot\\Members\\";
            string sessionFolderPath = Path.Combine(signedFolderPath, sessionValue);
            bool sessionFolderExists = Directory.Exists(sessionFolderPath);

            if (sessionFolderExists && Directory.GetFiles(sessionFolderPath).Length <= 3)
            {
                return Redirect("/QRCode/Yonlendirme");
            }

            string folderPath = "C:\\Users\\melih_o\\Downloads\\sayfaASP-master\\sayfaASP-master\\WebApplication1\\wwwroot\\Members\\SignedPDFs\\";
            string sFolderPath = Path.Combine(folderPath, sessionValue);
            bool folderExists = Directory.Exists(sFolderPath);

            if (!folderExists)
            {
                return Redirect("/QRCode/Yonelme");
            }

            //TcNo session değeri ile oturum açılmış ise yapılan işlemler sessionValue değişkenine kadar olanlar

            var usersTc = (from u in _context.Users
                           join ui in _context.UsersInfo on u.Name equals ui.NameId
                           join uu in _context.UsersUnit on u.Unit equals uu.UnitId
                           join uo in _context.UsersCom on u.Com equals uo.ComId
                           where u.tcNo == sessionTcNo
                           select new UserViewModel
                           {
                               Id = u.Id,
                               taxNo = u.taxNo,
                               tcNo = u.tcNo,
                               ComAd = uo.ComAd,
                               Email = u.Email,
                               UnitAd = uu.UnitAd,
                               NameUser = ui.NameUser,
                               Surname = ui.Surname,
                               Phone = ui.Phone
                           }).ToList();

            ViewBag.UserList = usersTc;

            if (sessionTcNo != null)
            {
                // QR kodu oluşturma ve şifreleme TcNo için
                foreach (var user in usersTc)
                {
                    var encryptedTcNo = AESEncryption.Encrypt(user.tcNo);
                    var encryptedNameUser = AESEncryption.Encrypt(user.NameUser);
                    var encryptedUnit = AESEncryption.Encrypt(user.UnitAd);

                    var qrCodeText = $"{encryptedTcNo};{encryptedNameUser};{encryptedUnit}";

                    var qrCodeGenerator = new QRCodeGenerator();
                    var qrCodeData = qrCodeGenerator.CreateQrCode(qrCodeText, QRCodeGenerator.ECCLevel.Q);
                    var qrCode = new QRCode(qrCodeData);
                    var qrCodeImage = qrCode.GetGraphic(5);

                    using (var stream = new MemoryStream())
                    {
                        qrCodeImage.Save(stream, ImageFormat.Png);
                        var qrCodeBase64 = Convert.ToBase64String(stream.ToArray());
                        user.QRCodeBase64 = qrCodeBase64;
                    }
                }

                return View(usersTc);
            }

            //-------------------------------------------------------------//
            var users = (from u in _context.Users
                         join ui in _context.UsersInfo on u.Name equals ui.NameId
                         join uu in _context.UsersUnit on u.Unit equals uu.UnitId
                         join uo in _context.UsersCom on u.Com equals uo.ComId
                         where u.taxNo == sessionValue
                         select new UserViewModel
                         {
                             Id = u.Id,
                             taxNo = u.taxNo,
                             tcNo = u.tcNo,
                             ComAd = uo.ComAd,
                             Email = u.Email,
                             UnitAd = uu.UnitAd,
                             NameUser = ui.NameUser,
                             Surname = ui.Surname,
                             Phone = ui.Phone
                         }).ToList();

            ViewBag.UserList = users;

            // QR kodu oluşturma ve şifreleme
            foreach (var user in users)
            {
                var encryptedTcNo = AESEncryption.Encrypt(user.tcNo);
                var encryptedNameUser = AESEncryption.Encrypt(user.NameUser);
                var encryptedUnit = AESEncryption.Encrypt(user.UnitAd);

                var qrCodeText = $"{encryptedTcNo};{encryptedNameUser};{encryptedUnit}";

                var qrCodeGenerator = new QRCodeGenerator();
                var qrCodeData = qrCodeGenerator.CreateQrCode(qrCodeText, QRCodeGenerator.ECCLevel.Q);
                var qrCode = new QRCode(qrCodeData);
                var qrCodeImage = qrCode.GetGraphic(5);

                using (var stream = new MemoryStream())
                {
                    qrCodeImage.Save(stream, ImageFormat.Png);
                    var qrCodeBase64 = Convert.ToBase64String(stream.ToArray());
                    user.QRCodeBase64 = qrCodeBase64;
                }
            }

            return View(users);
        }
    }

    public class AESEncryption
    {
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("1234567890123456"); // Şifreleme için kullanılacak anahtar (16 byte uzunluğunda)

        public static string Encrypt(string plainText)
        {
            byte[] encryptedBytes;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Key;
                aes.Mode = CipherMode.ECB; // Use ECB cipher mode
                aes.Padding = PaddingMode.PKCS7; // Use PKCS7 padding
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        cs.Write(plainTextBytes, 0, plainTextBytes.Length);
                    }

                    encryptedBytes = ms.ToArray();
                }
            }

            return Convert.ToBase64String(encryptedBytes);
        }
    }
}
