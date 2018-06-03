using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Books_WebAPI.DTOs
{
    public class BookDto
    {
        public string Title { set; get; }
        public string Genre { get; set; }
        public string Author { get; set; }
    }
}