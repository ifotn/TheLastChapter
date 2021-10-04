using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TheLastChapter.Models
{
    public class Book
    {
        public int BookId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Author { get; set; }

        [Required]
        public string Title { get; set; }

        public string Image { get; set; }

        [Range(0.01, 999999)]
        public double Price { get; set; }
        public bool MatureContent { get; set; }
        public int CategoryId { get; set; }
    }
}
