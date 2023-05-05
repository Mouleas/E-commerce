using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Shopee.Models
{
    [Index(nameof(AdminEmail), IsUnique = true)]
    public class AdminModel
    {
        [Key]
        public int AdminId { get; set; }

        [Required]
        public string AdminEmail { get; set; }

        [Required]
        public string AdminPwd { get; set; }
    }
}
