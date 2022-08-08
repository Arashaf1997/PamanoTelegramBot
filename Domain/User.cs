using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    [Table("Users", Schema = "dbo")]
    public class User : BaseModel
    {
        public User()
        {
        }
        [Required(ErrorMessage = "Username cant be null.")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Password cant be null.")]
        public string Password { get; set; }
        public string EmailAddress { get; set; }
    }
}
