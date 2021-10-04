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

        //FK	
        public int OrderId { get; set; }

        public int BookId { get; set; }

        //FK
        public double Price { get; set; }

        public int Quantity { get; set; }

    }
}
