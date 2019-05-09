using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Northwind.Models
{
    public class OrderDetail
    {
        public int OrderID { get; set; }
      [Required(ErrorMessage = "OrderID is required")][Key]
        public Order order { get; set; }
        public int ProductID { get; set; }
        [Key]
        public Product product { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Discount { get; set; }
    }
}
