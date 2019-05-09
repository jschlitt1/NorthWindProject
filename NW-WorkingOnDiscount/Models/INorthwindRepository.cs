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

        void AddCustomer(Customer customer);
        void EditCustomer(Customer customer);
        void RemoveItem(CartItem cartItem);
        void UpdateQuantity(CartItem cartItem);
        void UpdateInStock(Product product);
        void ApplyDiscount(CartItem cartItem, int discountCode);

        CartItem AddToCart(CartItemJSON cartItemJSON);
        

    }
}
