using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.Data.SqlClient;

namespace AESDecryptionExample
{
    public partial class Form1 : Form
    {
        private string connectionString = "Server=localhost;Database=login;Trusted_Connection=True;Encrypt=false";
        private string aesKey = "1234567890123456";

        public Form1()
        {
            InitializeComponent();
        }

        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string barkodVerisi = textBox.Text;

                try
                {
                    string processedBarcode = ProcessBarcode(barkodVerisi);
                    string[] sifreliMetinler = processedBarcode.Split(';');
                    List<string> cozulenMetinler = new List<string>();

                    foreach (string sifreliMetin in sifreliMetinler)
                    {
                        byte[] qrKod = Convert.FromBase64String(sifreliMetin);
                        string sifresizVeri = Decrypt(qrKod, aesKey);
                        cozulenMetinler.Add(sifresizVeri);
                    }

                    listBox.Items.Clear();
                    listBox.Items.AddRange(cozulenMetinler.ToArray());

                    bool girisBasarili = false;

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        foreach (var metin in cozulenMetinler)
                        {
                            SqlCommand command = new SqlCommand("SELECT Users.tcNo, UsersInfo.NameUser, UsersUnit.UnitAd FROM Users JOIN UsersInfo ON UsersInfo.NameId = Users.Name JOIN UsersUnit ON UsersUnit.UnitId = Users.Unit WHERE Users.tcNo = @tcNo", connection);
                            command.Parameters.AddWithValue("@tcNo", metin);

                            SqlDataReader reader = command.ExecuteReader();
                            if (reader.Read())
                            {
                                MessageBox.Show("Giriş Başarılı");
                                girisBasarili = true;
                                break;
                            }

                            reader.Close();
                        }

                        connection.Close();
                    }

                    if (!girisBasarili)
                    {
                        MessageBox.Show("Giriş Başarısız");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }

                textBox.Clear();
            }
        }

        private string ProcessBarcode(string barcode)
        {
            barcode = barcode.Replace("-", "=");
            barcode = barcode.Replace(".", "/");
            barcode = barcode.Replace("ı", "i");
            barcode = barcode.Replace("ş", ";");
            return barcode;
        }

        private string Decrypt(byte[] encryptedBytes, string key)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);

            using (Aes aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;

                using (ICryptoTransform decryptor = aes.CreateDecryptor())
                {
                    byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                    string decryptedText = Encoding.UTF8.GetString(decryptedBytes);
                    return decryptedText;
                }
            }
        }
    }
}
