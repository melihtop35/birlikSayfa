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

            // QR kodu oluşturma ve şifreleme
            foreach (var user in users)
            {
                var encryptedTcNo = AESEncryption.Encrypt(user.tcNo);

                var qrCodeGenerator = new QRCodeGenerator();
                var qrCodeData = qrCodeGenerator.CreateQrCode(encryptedTcNo, QRCodeGenerator.ECCLevel.Q);
                var qrCode = new QRCode(qrCodeData);
                var qrCodeImage = qrCode.GetGraphic(5);

                using (var stream = new MemoryStream())
                {
                    qrCodeImage.Save(stream, ImageFormat.Png);
                    var qrCodeBase64 = Convert.ToBase64String(stream.ToArray());
                    user.QRCodeBase64 = qrCodeBase64;
                }

                // Şifreli tcNo'yu çöz
                var decryptedTcNo = AESDecryption.Decrypt(encryptedTcNo);
                user.DecryptedTcNo = decryptedTcNo;
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
        byte[] iv;

        using (Aes aes = Aes.Create())
        {
            aes.Key = Key;
            aes.GenerateIV();
            iv = aes.IV;

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, iv);

            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(iv, 0, iv.Length);

                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    cs.Write(plainTextBytes, 0, plainTextBytes.Length);
                }

                encryptedBytes = ms.ToArray();
            }
        }

        byte[] encryptedBytesWithIV = new byte[iv.Length + encryptedBytes.Length];
        Buffer.BlockCopy(iv, 0, encryptedBytesWithIV, 0, iv.Length);
        Buffer.BlockCopy(encryptedBytes, 0, encryptedBytesWithIV, iv.Length, encryptedBytes.Length);

        return Convert.ToBase64String(encryptedBytesWithIV);
    }
}


    public class AESDecryption
    {
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("1234567890123456"); // Şifreleme için kullanılan aynı anahtar
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("1234567890123456"); // Şifreleme için kullanılan aynı IV vektörü

        public static string Decrypt(string encryptedText)
        {
            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
            string decryptedText;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream(encryptedBytes))
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader reader = new StreamReader(cs))
                        {
                            decryptedText = reader.ReadToEnd();
                        }
                    }
                }
            }

            return decryptedText;
        }
    }
}
