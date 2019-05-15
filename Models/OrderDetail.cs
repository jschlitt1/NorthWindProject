using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Northwind.Models
{
    public class OrderDetail
    {
        [Key, Column(Order = 0)]
        public int OrderID { get; set; }
      [Required(ErrorMessage = "OrderID is required")]
        //public Order order { get; set; }
        [Key, Column(Order = 1)]
        public int ProductID { get; set; }
        public Product product { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Discount { get; set; }
    }
}
