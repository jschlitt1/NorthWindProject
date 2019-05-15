using System.Linq;

namespace Northwind.Models
{
    public interface INorthwindRepository
    {
        IQueryable<Category> Categories { get; }
        IQueryable<Product> Products { get; }
        IQueryable<Discount> Discounts { get; }
        IQueryable<Customer> Customers { get; }
        IQueryable<CartItem> CartItems { get; }
        IQueryable<Order> Orders { get; }
        IQueryable<OrderDetail> OrderDetails { get; }
        IQueryable<Shipper> Shippers { get; }

        void AddCustomer(Customer customer);
        void EditCustomer(Customer customer);
        void RemoveItem(CartItem cartItem);
        void UpdateQuantity(CartItem cartItem);
        void UpdateInStock(Product product, int qty);
        void CheckDiscount(Discount discount);
        int CreateOrder(Order order);
        void CreateOrderDetail(OrderDetail orderDetail);
        decimal ApplyDiscount(int customerId, Discount discount, Product product);

        CartItem AddToCart(CartItemJSON cartItemJSON);
        

    }
}
