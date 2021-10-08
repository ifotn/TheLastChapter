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
        [MaxLength(500)]
        public string Title { get; set; }

        public string Image { get; set; }

        [Range(0.01, 999999)]
        [DisplayFormat(DataFormatString = "{0:c}")]  // c = MS currency format
        public double Price { get; set; }

        [Display(Name = "Mature Content")]
        public bool MatureContent { get; set; }

        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        // child refs
        public List<CartItem> CartItems { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }

        // parent ref
        public Category Category { get; set; }
    }
}
