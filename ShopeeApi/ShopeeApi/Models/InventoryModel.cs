using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ShopeeApi.Model
{
    [Index(nameof(ItemName), IsUnique = true)]
    public class InventoryModel
    {
        [Key]
        public int ItemId { get; set; }
        [Required]
        public string ItemName { get; set; }
        [Required]
        public string ItemType { get; set; }
        [Required]
        public string ItemPrice { get; set; }
        [Required]
        public string ItemDescription { get; set; }
        [Required]
        public int ItemQuantity { get; set; }
        [Required]
        public string ItemImageName { get; set; }
        public int ItemQuantitySelected { get; set; } = 0;

        public virtual ICollection<CartModel> Carts { get; set; }

        public virtual ICollection<ForumModel> Forums { get; set; }
    }
}
