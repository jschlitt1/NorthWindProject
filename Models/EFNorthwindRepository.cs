﻿using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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
        public IQueryable<Order> Orders => context.Orders;
        public IQueryable<OrderDetail> OrderDetails => context.OrderDetails;
        public IQueryable<Shipper> Shippers => context.Shippers;

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

        public void UpdateInStock(Product product, int Qty)
        {
            product.UnitsInStock = (short)(product.UnitsInStock - Qty);
        }
        public void CheckDiscount(Discount discount)
        {
        }
        public int CreateOrder(Order order)
        {
            //int orderId = order.OrderID;
            context.Orders.Add(order);
            context.SaveChanges();
            int orderId = context.Orders.FirstOrDefault(o => o.CustomerID == order.CustomerID).OrderID;
            return orderId;
        }
        public void CreateOrderDetail(OrderDetail orderDetail)
        {
            context.OrderDetails.Add(orderDetail);
            context.SaveChanges();
        }
        public decimal ApplyDiscount(int customerId, Discount discount, Product product)
        {
            //apply discount
            decimal productPrice = product.UnitPrice;
            decimal discountAmount = productPrice * discount.DiscountPercent;

            //get total
            IQueryable<decimal> unitPrices = context.CartItems
                .Include("Product")
                .Where(u => u.CustomerId == customerId)
                .Select(p => p.Product.UnitPrice);

            IQueryable<int> quantity = context.CartItems
                .Where(u => u.CustomerId == customerId)
                .Select(c => c.Quantity);

            decimal[] unitPriceList = unitPrices.ToArray();
            int[] quantityList = quantity.ToArray();

            List<decimal> eachItemTotal = new List<decimal>();

            for (int i = 0; i < unitPriceList.Length; i++)
            {
                eachItemTotal.Add(unitPriceList[i] * quantityList[i]);
            }

            decimal total = eachItemTotal.Sum();
            total = total - discountAmount;

            return total;
        }
    }
}
