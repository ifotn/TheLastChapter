using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TheLastChapter.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        public string CustomerId { get; set; }
        public DateTime PurchaseDate { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string City { get; set;  }

        [Required]
        [MaxLength(2)]
        public string Province { get; set; }

        [Required]
        [MaxLength(7)]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }

        [Required]
        [MaxLength(15)]
        public string Phone { get; set; }  // 1-800-Buy-Crap

        public string PaymentCode { get; set; }

        // child ref
        public List<OrderDetail> OrderDetails { get; set; }
    }
}
