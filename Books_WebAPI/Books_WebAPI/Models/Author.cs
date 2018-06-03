using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Books_WebAPI.Models
{
    public class Author
    {
        [Key]
        public int AuthorId { get; set; }
        [Required]
        public string Name { get; set; }
    }
}