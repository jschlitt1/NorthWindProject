using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Northwind.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        [Required(ErrorMessage = "OrderID is required")]
        public DateTime orderDate { get; set; }
        public DateTime RequiredDate { get; set; }
        public DateTime ShippedDate { get; set; }
        public decimal Freight { get; set; }
        public string ShipName { get; set; }
        public string ShipAddress { get; set; }
        public string ShipCity { get; set; }
        public string ShipRegion { get; set; }
        public string ShipPostalCode { get; set; }
        public string ShipCountry { get; set; }

        [ForeignKey("ShipperID")]
        public int ShipVia { get; set; }        
        public Shipper shipper{ get; set; }

        public int CustomerID { get; set; }
        public Customer customer { get; set; }

        public int EmployeeID { get; set; }
        public Employee employee { get; set; }
    }
}

