using System.ComponentModel.DataAnnotations.Schema;

namespace ShopeeApi.Dao
{
    public class DaoCart
    {
        public int UserId { get; set; }
        public int InventoryId { get; set; }
        public int Quantity { get; set; }
    }
}
