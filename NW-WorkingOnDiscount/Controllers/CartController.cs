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

            IQueryable<decimal> prices = repository.CartItems
                .Include("Product")
                .Where(u => u.CustomerId == customerId)
                .Select(p => p.Product.UnitPrice);

            IQueryable<int> quantity = repository.CartItems
                .Where(u => u.CustomerId == customerId)
                .Select(c => c.Quantity);

            decimal[] priceList = prices.ToArray();
            int[] quantityList = quantity.ToArray();

            List<decimal> eachItemTotal = new List<decimal>();

            for (int i = 0; i < priceList.Length; i++)
            {
                eachItemTotal.Add(priceList[i]*quantityList[i]);
            }

            ViewBag.EachItem = eachItemTotal.ToArray();
            decimal total = eachItemTotal.Sum();
            ViewBag.Total = total;

            return View(repository.CartItems.Include("Product").Where(u => u.CustomerId == customerId));            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //needs to be updated
        public IActionResult CartList(int Code)
        {
            int customerId = repository.Customers.FirstOrDefault(c => c.Email == User.Identity.Name).CustomerID;
            bool valid = false;
            //apply after
            //int[] AppliedDiscounts;

            Discount discount = repository.Discounts.Include("Product").FirstOrDefault(d => d.Code == Code);
            //testing for invalid code
            if (discount == null)
            {
                //tell user code is bad
                ModelState.AddModelError("", "The code is not available");

            }
            //check date
            else if (discount.EndTime <= DateTime.Now)
            {
                //tell user discount is outdated
                ModelState.AddModelError("", "The code is out-dated");

            }
            else
            {
                //checking to make sure the item is in the cart
                foreach (CartItem i in repository.CartItems.Where(c => c.CustomerId == customerId))
                {
                    if (discount.ProductID == i.ProductId)
                    {
                        valid = true;
                        break;
                    }
                    else
                    {
                        //tell user that the item they have is not valid
                        ModelState.AddModelError("", "Cannot apply the code to your cart");
                        break;
                    }
                }

            }
            if (valid)
            {
                //int customerId = repository.Customers.FirstOrDefault(c => c.Email == User.Identity.Name).CustomerID;
                //apply ApplyDiscount method
                //repository.ApplyDiscount();

                IQueryable<decimal> prices = repository.CartItems
                    .Include("Product")
                    .Where(u => u.CustomerId == customerId)
                    .Select(p => p.Product.UnitPrice);

                decimal[] priceList = prices.ToArray();
                decimal total = priceList.Sum();
                //decimal total = ViewBag.Total;
                //where discount.ProductId take that product, look at price
                //will need include for product
                decimal ProductPrice = discount.Product.UnitPrice;
                decimal discountAmount = ProductPrice * discount.DiscountPercent;
                total = total - discountAmount;
                ViewBag.total = total;

                //itemPrice * discount = amount off
                //take amount off from total
                return RedirectToAction("CartList");
            }
            //needs to recived the changed discount
            //repository.CheckDiscount(discount);
            return View(repository.CartItems.Include("Product").Where(u => u.CustomerId == customerId));


        }

        [HttpPost]
        public IActionResult RemoveItem(int id)
        {
            repository.RemoveItem(repository.CartItems.FirstOrDefault(i => i.CartItemId == id));
            return RedirectToAction("CartList");
        }
        [HttpPost]
        public IActionResult UpdateQuantity(int? id, int Quantity)
        {
            CartItem cartItem = repository.CartItems.FirstOrDefault(ci => ci.CartItemId == id);
            cartItem.Quantity = Quantity;
            repository.UpdateQuantity(cartItem);
            return RedirectToAction("CartList");
        }
    }
}
