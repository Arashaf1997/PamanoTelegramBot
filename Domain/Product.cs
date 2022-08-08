using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    [Table("Products", Schema = "dbo")]
    public class Product : BaseModel
    {
        public Product()
        {
        }
        public string Name { get; set; }
        public int Price { get; set; }
        public string Description { get; set; }


        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public virtual User User { get; set; }
    }
}
