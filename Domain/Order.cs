using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    [Table("Orders", Schema = "dbo")]
    public class Order : BaseModel
    {
        public Order()
        {
        }
        public int TotalPrice { get; set; }
        public int TotalCount { get; set; }
        public string Details { get; set; }
        public string CustomerDescription { get; set; }

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public virtual User User { get; set; }

        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
    }
}
