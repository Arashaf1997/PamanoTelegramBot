using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    [Table("ProductImages", Schema = "dbo")]
    public class ProductImage : BaseModel
    {
        public ProductImage()
        {
        }
        public int ProductId { get; set; }
        public string ImageId { get; set; }
    }
}
