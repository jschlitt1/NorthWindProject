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

            //decimal[] CartItemPrices = repository.CartItems
            //    .Include("Product")
            //    .Where(u => u.CustomerId == customerId)
            //    .Select(p => new { p.Product.UnitPrice })
            //    .ToArray();

            ////foreach (cartItem i in Model)


            //var total = CartItemPrices.Sum();

            //ViewBag.Total = total;


            return View(repository.CartItems.Include("Product").Where(u => u.CustomerId == customerId));
        }

        [HttpPost]
        public IActionResult RemoveItem(int id)
        {
            repository.RemoveItem(repository.CartItems.FirstOrDefault(i => i.CartItemId == id));
            return RedirectToAction("CartList");
        }
        //[HttpPost]
        //public IActionResult UpdateQuantity(int id)
        //{
        //    CartItem cartItem = repository.CartItems.FirstOrDefault(c => c.CartItemId == id);
        //    //repository.EditCustomer(customer);
        //    repository.UpdateQuantity(cartItem);
        //    return RedirectToAction("CartList");
        //}
        [HttpPost]
        public IActionResult UpdateQuantity(int? id, int Quantity)
        {
            CartItem cartItem = repository.CartItems.FirstOrDefault(ci => ci.CartItemId == id);
            cartItem.Quantity = Quantity;
            repository.UpdateQuantity(cartItem);
            return RedirectToAction("CartList");
        }
        [HttpPost]
        //needs to be updated
        public IActionResult ApplyDiscount(string Code)
        {
            Discount discount = repository.Discounts.FirstOrDefault(d => d.Code == Code);
            //needs to recived the changed discount
            repository.CheckDiscount(discount);
            return RedirectToAction("CartList");
        }
    }
}
