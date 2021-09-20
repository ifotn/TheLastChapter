using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheLastChapter.Models
{
    public class Category
    {
        // pk fields should always be called either {Model}Id or just Id
        public int CategoryId { get; set; }
        public string Name { get; set; }
    }
}
