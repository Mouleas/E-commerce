using System.ComponentModel.DataAnnotations.Schema;

namespace ShopeeApi.Dao
{
    public class DaoForum
    {
        public int ItemId { get; set; }
        public int UserId { get; set; }
        public string ForumSubject { get; set; }
        public string ForumBody { get; set; }
        public int ForumStatus { get; set; }
    }
}
