using System;
using System.ComponentModel.DataAnnotations;

namespace Northwind.Models
{
    public class Shipper
    {
        public int ShipperID { get; set; }
        [Required(ErrorMessage ="ShipperID is required")]
        public string CompanyName { get; set; }
        [Required(ErrorMessage ="Company Name is required")]
        public string Phone { get; set; }
    }
}
