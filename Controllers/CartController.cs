using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Northwind.Models;

namespace Northwind.Controllers
{
    public class CartController : Controller
    {
        // this controller depends on the NorthwindRepository
        private INorthwindRepository repository;
        public CartController(INorthwindRepository repo) => repository = repo;

        public ActionResult CartList()
        {
            int customerId = repository.Customers.FirstOrDefault(c => c.Email == User.Identity.Name).CustomerID;
            return View(repository.CartItems.Include("Product").Where(u => u.CustomerId == customerId));
        }

        [HttpPost]
        public IActionResult RemoveItem(int id)
        {
            repository.RemoveItem(repository.CartItems.FirstOrDefault(i => i.CartItemId == id));
            return RedirectToAction("CartList");
        }
        [HttpPost]
        public IActionResult UpdateQuantity(int id)
        {
            CartItem cartItem = repository.CartItems.FirstOrDefault(c => c.CartItemId == id);
            //repository.EditCustomer(customer);
            repository.UpdateQuantity(cartItem);
            return RedirectToAction("CartList");
        }
        //[HttpPost]
        //public IActionResult UpdateQuantity(CartItem cartItem)
        //{
        //    repository.UpdateQuantity(cartItem);
        //    return RedirectToAction("CartList");
        //}
    }
}
