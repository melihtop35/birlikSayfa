using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace WebApplication1.Models
{
    public class UsersUnit
    {
        [Key]
        public int UnitId { get; set; }
        public string UnitAd { get; set; }
    }
}
