using Microsoft.AspNetCore.Mvc;
using QRCoder;
using System.Drawing.Imaging;
using System.Drawing;

namespace WebApplication1.Controllers
{
    public class QRCodeController : Controller
    {
        public IActionResult QRCode()
        {
            string folderPath = "C:\\Users\\melih_o\\Downloads\\sayfaASP-master\\sayfaASP-master\\WebApplication1\\wwwroot\\NewFolder";
            DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);
            List<string> filePaths = new List<string>();

            foreach (var file in directoryInfo.GetFiles())
            {
                if (file.Extension == ".pdf")
                {
                    filePaths.Add(file.FullName);
                }
            }

            List<string> qrCodeBase64Images = new List<string>();
            foreach (var filePath in filePaths)
            {
                // QR kodu içeriğini PDF dosyasının yoluna ayarlayın
                string qrCodeContent = filePath;

                // QR kod oluşturucu nesnesini oluşturun
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrCodeContent, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);

                // QR kodu bir görüntü olarak kaydedin
                Bitmap qrCodeImage = qrCode.GetGraphic(20); // 20 piksel genişlik

                // QR kod görüntüsünü MemoryStream'e yazdırın
                MemoryStream ms = new MemoryStream();
                qrCodeImage.Save(ms, ImageFormat.Png);
                ms.Seek(0, SeekOrigin.Begin);

                // Görüntüyü Base64 string'e çevirin ve listeye ekleyin
                qrCodeBase64Images.Add("data:image/png;base64," + Convert.ToBase64String(ms.ToArray()));
            }

            // QR kodlu imzalı PDF dosyalarının listesini görüntüleyen sayfaya gönderin
            return View(qrCodeBase64Images);
        }
    }
}
