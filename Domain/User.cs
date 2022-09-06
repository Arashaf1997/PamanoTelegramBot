﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    [Table("Users", Schema = "dbo")]
    public class User : BaseModel
    {
        public User()
        {
        }
        public long ChatId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string StoreName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
    }
}
