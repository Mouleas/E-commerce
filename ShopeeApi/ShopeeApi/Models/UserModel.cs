using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ShopeeApi.Model
{
    [Index(nameof(UserEmail), IsUnique = true)]
    public class UserModel
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string UserAddress { get; set; }

        [Required]
        public string UserEmail { get; set; }

        [Required]
        public string UserPassword { get; set; }

        public virtual ICollection<CartModel> Carts { get; set; }

        public virtual ICollection<ForumModel> Forums { get; set; }
    }
}
