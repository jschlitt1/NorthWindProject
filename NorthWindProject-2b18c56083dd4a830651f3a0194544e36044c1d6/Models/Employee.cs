using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Northwind.Models
{
    public class Employee
    {
        public int EmployeeID { get; set; }
        [Required(ErrorMessage = "EmployeeID is required")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Last Name is required")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "First Name is required")]
        public string Title { get; set; }
        public string TitleOfCoutesy { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime HireDate { get; set; }
        public string Address { get; set; }
        public string city { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string HomePhone { get; set; }
        public string Extension { get; set; }

        public int ReportsTo { get; set; }
        public Employee employee { get; set; }
    }
}
