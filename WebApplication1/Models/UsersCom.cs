using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class UsersCom
    {
        [Key]
        public int ComId { get; set; }
        public string ComAd { get; set; }
    }
}
