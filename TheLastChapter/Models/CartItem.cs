using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheLastChapter.Models
{
    public class CartItem
    {
        //pk
        public int CartItemId { get; set; }

        public double Price { get; set; }

        public int Quantity { get; set; }

        public int BookId { get; set; } // fk

        public DateTime DateAdded { get; set; }

        public string CustomerId { get; set; }

        // parent ref
        public Book Book { get; set; }
    }
}
