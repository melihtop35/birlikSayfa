using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string taxNo { get; set; }
        public string tcNo { get; set; }
        public string Email { get; set; }
        public int Name { get; set; }
        public int Unit { get; set; }
    }
}
