using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shopee.Models
{
    public class ForumModel
    {
        [Key]
        public int ForumId { get; set; }
        [ForeignKey("Inventory")]
        public int ItemId { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        [Required]
        public string ForumSubject { get; set; }
        [Required]
        public string ForumBody { get; set; }
        public string NewMessage { get; set; }
        public int ForumStatus { get; set; }
        public virtual UserModel User { get; set; }
        public virtual InventoryModel Inventory { get; set; }
    }
}
