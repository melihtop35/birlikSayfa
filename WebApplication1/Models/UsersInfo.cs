using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace WebApplication1.Models
{
    public class UsersInfo
    {
        [Key]
        public int NameId { get; set; }
        public string NameUser { get; set; }
        public string Surname { get; set; }
        public string Phone { get; set; }
    }
}
