using Microsoft.AspNetCore.Mvc;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.IO;

public class ESignatureController : Controller
{
    public IActionResult ESignature()
    {
        return View();
    }

    public IActionResult SignDocument()
    {
        return View();
    }
}
