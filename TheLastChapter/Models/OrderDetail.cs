using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheLastChapter.Models
{
    public class OrderDetail
    {
        //PK	
        public int OrderDetailId { get; set; }

        //FKs	
        public int OrderId { get; set; }

        public int BookId { get; set; }


        public double Price { get; set; }

        public int Quantity { get; set; }

        // parent refs
        public Book Book { get; set; }
        public Order Order { get; set; }
    }
}
