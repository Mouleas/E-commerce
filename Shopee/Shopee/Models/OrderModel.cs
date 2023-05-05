using System.ComponentModel.DataAnnotations;

namespace Shopee.Models
{
    public class OrderModel
    {
        [Key]
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public string ItemName { get; set; }
        public string ItemType { get; set; }
        public int Quantity { get; set; }
        public int TotalAmount { get; set; }
        public DateTime Date { get; set; }
    }
}
