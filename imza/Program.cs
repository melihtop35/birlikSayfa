using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;

public class SignatureProgram
{
    private string folderPath;
    private List<string> pdfFiles;
    private List<string> selectedFiles;

    public SignatureProgram(string folderPath)
    {
        this.folderPath = folderPath;
        pdfFiles = Directory.GetFiles(folderPath, "*.pdf").ToList();
        selectedFiles = new List<string>();
    }

    public void Run()
    {
        bool exit = false;

        while (!exit)
        {
            Console.Clear();
            Console.WriteLine("=== E-İmza Programı ===");
            Console.WriteLine("1. Klasörleri Listele");
            Console.WriteLine("2. Klasör Seç ve Dosyaları Listele");
            Console.WriteLine("3. Dosya Seç");
            Console.WriteLine("4. Seçilen Dosyaları Aç");
            Console.WriteLine("5. Seçilen Dosyaları İmzala");
            Console.WriteLine("6. Çıkış");
            Console.Write("Seçiminizi yapın (1-6): ");

            string input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    ListFolders();
                    break;
                case "2":
                    SelectFolderAndListFiles();
                    break;
                case "3":
                    SelectFiles();
                    break;
                case "4":
                    OpenSelectedFiles();
                    break;
                case "5":
                    SignSelectedFiles();
                    break;
                case "6":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Geçersiz seçim. Lütfen tekrar deneyin.");
                    break;
            }

            Console.WriteLine();
            Console.WriteLine("Devam etmek için bir tuşa basın...");
            Console.ReadKey();
        }
    }

    private void ListFolders()
    {
        Console.WriteLine("=== Klasörler ===");
        string[] folders = Directory.GetDirectories(folderPath);

        if (folders.Any())
        {
            for (int i = 0; i < folders.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {Path.GetFileName(folders[i])}");
            }
        }
        else
        {
            Console.WriteLine("Klasörde hiç alt klasör bulunamadı.");
        }
    }

    private void SelectFolderAndListFiles()
    {
        Console.WriteLine("=== Klasör Seç ve Dosyaları Listele ===");
        string[] folders = Directory.GetDirectories(folderPath);

        if (folders.Any())
        {
            Console.WriteLine("Klasör numarasını girin:");
            string input = Console.ReadLine();
            if (int.TryParse(input, out int folderIndex) && folderIndex > 0 && folderIndex <= folders.Length)
            {
                string selectedFolder = folders[folderIndex - 1];
                pdfFiles = Directory.GetFiles(selectedFolder, "*.pdf").ToList();
                Console.WriteLine("=== Dosyalar ===");
                if (pdfFiles.Any())
                {
                    for (int i = 0; i < pdfFiles.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {Path.GetFileName(pdfFiles[i])}");
                    }
                }
                else
                {
                    Console.WriteLine("Seçilen klasörde hiç PDF dosyası bulunamadı.");
                }
            }
            else
            {
                Console.WriteLine($"Geçersiz klasör numarası: {input}");
            }
        }
        else
        {
            Console.WriteLine("Klasörde hiç alt klasör bulunamadı.");
        }
    }

    private void SelectFiles()
    {
        Console.WriteLine("=== Dosya Seç ===");
        if (pdfFiles.Any())
        {
            Console.WriteLine("Dosya numaralarını virgülle ayırarak girin (örn: 1,3,4):");
            string input = Console.ReadLine();
            string[] selections = input.Split(',');

            foreach (string selection in selections)
            {
                if (int.TryParse(selection, out int fileIndex) && fileIndex > 0 && fileIndex <= pdfFiles.Count)
                {
                    string selectedFile = pdfFiles[fileIndex - 1];
                    selectedFiles.Add(selectedFile);
                    Console.WriteLine($"Dosya seçildi: {Path.GetFileName(selectedFile)}");
                }
                else
                {
                    Console.WriteLine($"Geçersiz dosya numarası: {selection}");
                }
            }
        }
        else
        {
            Console.WriteLine("Henüz hiç dosya bulunamadı.");
        }
    }

    private void OpenSelectedFiles()
    {
        Console.WriteLine("=== Seçilen Dosyaları Aç ===");
        if (selectedFiles.Any())
        {
            foreach (string selectedFile in selectedFiles)
            {
                Console.WriteLine($"Dosya açılıyor: {Path.GetFileName(selectedFile)}");

                // Seçili dosyayı tarayıcıda aç
                Process.Start(selectedFile);
            }
        }
        else
        {
            Console.WriteLine("Henüz hiç dosya seçilmedi.");
        }
    }


    private void SignSelectedFiles()
    {
        Console.WriteLine("=== Seçilen Dosyaları İmzala ===");
        if (selectedFiles.Any())
        {
            foreach (string selectedFile in selectedFiles)
            {
                Console.WriteLine($"Dosya imzalanıyor: {Path.GetFileName(selectedFile)}");

                // Seçilen klasör adını al
                string selectedFolderName = Path.GetFileName(Path.GetDirectoryName(selectedFile));

                // İmzalanan dosyaları yerleştireceğimiz klasörü oluştur
                string signedFolderPath = Path.Combine(folderPath, "SignedPDFs", selectedFolderName);
                Directory.CreateDirectory(signedFolderPath);

                string signedFileName = "imzalı_" + Path.GetFileNameWithoutExtension(selectedFile) + Path.GetExtension(selectedFile);
                string signedFilePath = Path.Combine(signedFolderPath, signedFileName);

                using (iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(selectedFile))
                {
                    using (FileStream fs = new FileStream(signedFilePath, FileMode.Create, FileAccess.Write))
                    {
                        using (PdfStamper stamper = new PdfStamper(reader, fs))
                        {
                            // Yeni sayfa oluştur
                            PdfContentByte content = stamper.GetOverContent(1);

                            // Resim dosyasını yükleyin veya çizin (bu örnekte bir örnek resim kullanılmıştır)
                            Image image = Image.GetInstance("C:\\Users\\melih_o\\Downloads\\sayfaASP-master\\sayfaASP-master\\WebApplication1\\wwwroot\\images\\imza.png");

                            // Resmi PDF sayfasının sağ alt tarafına yerleştirin
                            image.SetAbsolutePosition(stamper.Reader.GetPageSize(1).Width - image.Width, 0);
                            content.AddImage(image);

                            // İmzalanan PDF'i kaydet
                            stamper.Close();
                        }
                    }
                }

                Console.WriteLine($"Dosya imzalandı ve kaydedildi: {Path.GetFileName(signedFilePath)}");

                // İmzalanan dosyayı members klasöründen sil
                File.Delete(selectedFile);
                Console.WriteLine($"Dosya silindi: {Path.GetFileName(selectedFile)}");
            }

            // Klasörde dosya kalmadıysa, ilgili klasörü sil
            string selectedFolder = Path.GetDirectoryName(selectedFiles.First());
            if (!Directory.EnumerateFileSystemEntries(selectedFolder).Any())
            {
                Directory.Delete(selectedFolder);
                Console.WriteLine($"Klasör silindi: {selectedFolder}");
            }

            Console.WriteLine("İşlem tamamlandı. İmzalanan PDF'ler, 'SignedPDFs' klasörünün içindeki ilgili klasöre yerleştirildi ve orijinal dosyalar silindi.");
        }
        else
        {
            Console.WriteLine("Henüz hiç dosya seçilmedi.");
        }
    }



}

class Program
{
    static void Main(string[] args)
    {
        string folderPath = "C:\\Users\\melih_o\\Downloads\\sayfaASP-master\\sayfaASP-master\\WebApplication1\\wwwroot\\Members\\";
        SignatureProgram program = new SignatureProgram(folderPath);
        program.Run();
    }
}
