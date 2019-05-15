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

            decimal[] priceList = prices.ToArray();
            decimal total = priceList.Sum();

            ViewBag.Total = total;

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

        [HttpPost]
        public IActionResult UpdateInStock(int? id, short Quantity)
        {
            Product product = repository.Products.FirstOrDefault(p => p.ProductId == id);
            product.UnitsInStock = Quantity;
            repository.UpdateInStock(product);
            return RedirectToAction("CartList");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CartList(int Code)
        {
            int customerId = repository.Customers.FirstOrDefault(c => c.Email == User.Identity.Name).CustomerID;
            bool valid = false;

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
                        valid = false;
                        ModelState.AddModelError("", "Cannot apply the code to your cart");
                    }
                    
                }

            }
            if(valid)
            {
                //apply ApplyDiscount method
                Product product = repository.Products.FirstOrDefault(p => p.ProductId == discount.ProductID);
                ViewBag.Total = repository.ApplyDiscount(customerId, discount, product);

                return RedirectToAction("CartList");
            }
                //needs to recived the changed discount
                repository.CheckDiscount(discount);
            return View(repository.CartItems.Include("Product").Where(u => u.CustomerId == customerId));


        }
        [HttpPost]
        public IActionResult Checkout()
        {
            int customerId = repository.Customers.FirstOrDefault(c => c.Email == User.Identity.Name).CustomerID;
        //    //this should only be done once per cart
            Order newOrder;
        //    //var newOrder = context.Orders.FirstOrDefault(o => o.CustomerID == order.CustomerID);
        //    //need orderID (autogen?)
        //    //CustomerID (from this user)
            newOrder.CustomerID = customerId;
        //    //EmployeeID (null)
        //    //OrderDate (set as the current date)
            newOrder.orderDate = DateTime.Now;
        //    //RequireDate (null)
        //    //ShippedDate (null)
        //    //ShipVia (null)
        //    //Freight (null)
        //    //ShipName (customer name)
            newOrder.ShipName = repository.Customers.FirstOrDefault(c => c.Email == User.Identity.Name).CompanyName;
        //    //ShipAddress (customer address)
            newOrder.ShipAddress = repository.Customers.FirstOrDefault(c => c.Email == User.Identity.Name).Address;
        //    //ShipCity (customer city)
            newOrder.ShipCity = repository.Customers.FirstOrDefault(c => c.Email == User.Identity.Name).City;
        //    //ShipRegion (customer region)
            newOrder.ShipRegion = repository.Customers.FirstOrDefault(c => c.Email == User.Identity.Name).Region;
        //    //ShipPostalCode (customer postal code)
            newOrder.ShipPostalCode = repository.Customers.FirstOrDefault(c => c.Email == User.Identity.Name).PostalCode;
        //    //ShipCountry (customer country)
            newOrder.ShipCountry = repository.Customers.FirstOrDefault(c => c.Email == User.Identity.Name).Country;
        //    //create with the info provided above
            int OrderID = CreateOrder(newOrder);
        //    //this will need to be done with every cart item, info needed listed below, need to create the model before sending it.
            foreach (CartItem i in repository.CartItems.Include("Product").Where(c => c.CustomerId == customerId))
            {
                OrderDetail newOd;
        //        //var newOd;
        //        //OrderID (recive from above)
                newOd.OrderID = OrderID;
        //        //ProductID (recive from cartItem)
                newOd.ProductID = i.ProductId;
        //        //UnitPrice (can follow example in code earlier to get this)
                newOd.UnitPrice = i.Product.UnitPrice;
        //        //quantity (recive from cartItem)
                newOd.Quantity = i.Quantity;
        //        //Discount (??, but used earlier)
                //newOd.Discount =
                CreateOrderDetail(newOd);
            }

            return RedirectToAction("Index");
        }
    }
}
