﻿using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Northwind.Models
{
    public class EFNorthwindRepository : INorthwindRepository
    {
        // the repository class depends on the NorthwindContext service
        // which was registered at application startup
        private NorthwindContext context;
        public EFNorthwindRepository(NorthwindContext ctx)
        {
            context = ctx;
        }
        // create IQueryable for Categories & Products
        public IQueryable<Category> Categories => context.Categories;
        public IQueryable<Product> Products => context.Products;
        public IQueryable<Discount> Discounts => context.Discounts;
        public IQueryable<Customer> Customers => context.Customers;
        public IQueryable<CartItem> CartItems => context.CartItems;

        public void AddCustomer(Customer customer)
        {
            context.Customers.Add(customer);
            context.SaveChanges();
        }

        public void EditCustomer(Customer customer)
        {
            var customerToUpdate = context.Customers.FirstOrDefault(c => c.CustomerID == customer.CustomerID);
            customerToUpdate.Address = customer.Address;
            customerToUpdate.City = customer.City;
            customerToUpdate.Region = customer.Region;
            customerToUpdate.PostalCode = customer.PostalCode;
            customerToUpdate.Country = customer.Country;
            customerToUpdate.Phone = customer.Phone;
            customerToUpdate.Fax = customer.Fax;
            context.SaveChanges();
        }


        public CartItem AddToCart(CartItemJSON cartItemJSON)
        {
            int CustomerId = context.Customers.FirstOrDefault(c => c.Email == cartItemJSON.email).CustomerID;
            int ProductId = cartItemJSON.id;
            // check for duplicate cart item
            CartItem cartItem = context.CartItems.FirstOrDefault(ci => ci.ProductId == ProductId && ci.CustomerId == CustomerId);
            if (cartItem == null)
            {
                // this is a new cart item
                cartItem = new CartItem()
                {
                    CustomerId = CustomerId,
                    ProductId = cartItemJSON.id,
                    Quantity = cartItemJSON.qty
                };
                context.Add(cartItem);
            }
            else
            {
                // for duplicate cart item, simply update the quantity
                cartItem.Quantity += cartItemJSON.qty;
            }

            context.SaveChanges();
            cartItem.Product = context.Products.Find(cartItem.ProductId);
            return cartItem;
        }

        public void RemoveItem(CartItem cartItem)
        {
            context.Remove(cartItem);
            context.SaveChanges();
        }
        public void UpdateQuantity(CartItem cartItem)
        {
            var CartItemToUpdate = context.CartItems.FirstOrDefault(c => c.CartItemId == cartItem.CartItemId);
            CartItemToUpdate.Quantity = cartItem.Quantity;
            context.SaveChanges();
        }

        public void UpdateInStock(Product product)
        {

        }
        //
        public void ApplyDiscount(CartItem cartItem, int code)
        {
            var discount = context.Discounts.Include("Product").FirstOrDefault(d => d.Code == code);
            
            IQueryable<decimal> prices = context.CartItems
                    .Include("Product")
                    .Where(u => u.CustomerId == cartItem.CustomerId)
                    .Select(p => p.Product.UnitPrice);

            decimal[] priceList = prices.ToArray();
            decimal total = priceList.Sum();
            //decimal total = ViewBag.Total;
            //where discount.ProductId take that product, look at price
            //will need include for product
            decimal ProductPrice = discount.Product.UnitPrice;
            decimal discountAmount = ProductPrice * discount.DiscountPercent;
            total = total - discountAmount;
            //ViewBag.total = total;

            //itemPrice * discount = amount off
            //take amount off from total
            //return RedirectToAction("CartList");
        }
    }
}