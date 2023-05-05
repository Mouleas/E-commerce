using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shopee.Models
{
    public class CartModel
    {
        [Key]
        public int CartId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        [ForeignKey("Inventory")]
        public int InventoryId { get; set; }
        public int Quantity { get; set; }

        public virtual UserModel User { get; set; }
        public virtual InventoryModel Inventory { get; set; }

    }
}
