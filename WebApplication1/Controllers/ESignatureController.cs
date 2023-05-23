using Microsoft.AspNetCore.Mvc;
using RestSharp;

public class ESignatureController : Controller
{
    public IActionResult ESignature()
    {
        return View();
    }

    public IActionResult SignDocument()
    {
        // E-imza servisine istek göndermek için RestSharp kullanıyoruz.
        var client = new RestClient("https://belge.etikimza.com/api/\r\n"); // E-imza servisinizin URL'sini buraya ekleyin

        var request = new RestRequest("sign", Method.Post);
        request.AddJsonBody(new { Document = "wwwroot\\NewFolder\\" }); // İmzalanacak belge içeriğini burada belirtin

        var response = client.Execute(request);

        if (response.IsSuccessful)
        {
            var signedDocument = response.Content; // İmzalanmış belge içeriğini alın
                                                   // İmzalanan belgeyle yapmak istediğiniz işlemleri gerçekleştirin
            ViewBag.SignedDocument = signedDocument;

            return View("SignedDocument");
        }
        else
        {
            return Redirect("/QRCode/QRCode");
        }
    }
}
