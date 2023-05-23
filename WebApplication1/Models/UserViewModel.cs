namespace WebApplication1.Models
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string ComAd { get; set; }
        public string taxNo { get; set; }
        public string tcNo { get; set; }
        public string Email { get; set; }
        public string UnitAd { get; set; }
        public string NameUser { get; set; }
        public string Surname { get; set; }
        public string Phone { get; set; }
        public string QRCodeBase64 { get; internal set; }
    }
}
