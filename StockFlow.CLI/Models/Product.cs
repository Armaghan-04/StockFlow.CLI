using System.Text.Json.Serialization;

namespace StockFlow.CLI.Models
{
    public class Product
    {
        [JsonInclude]
        public Guid Id { get; private set; } = Guid.NewGuid();

        public string Name { get; private set; } = "";

        public string SKU { get; private set; } = "";

        public decimal Price { get; private set; }

        public int Quantity { get; private set; }

        public Product(string name, string sku, decimal price, int quantity)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Product name is required.", nameof(name));

            if (string.IsNullOrWhiteSpace(sku))
                throw new ArgumentException("Product SKU is required.", nameof(sku));

            if (price <= 0)
                throw new ArgumentException("Product price must be greater than zero.", nameof(price));

            if (quantity < 0)
                throw new ArgumentException("Product quantity cannot be negative.", nameof(quantity));

            Name = name.Trim();
            SKU = sku.Trim();
            Price = price;
            Quantity = quantity;
        }

        [JsonConstructor]
        public Product(Guid id, string name, string sku, decimal price, int quantity)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Product ID cannot be empty.", nameof(id));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Product name is required.", nameof(name));

            if (string.IsNullOrWhiteSpace(sku))
                throw new ArgumentException("Product SKU is required.", nameof(sku));

            if (price <= 0)
                throw new ArgumentException("Product price must be greater than zero.", nameof(price));

            if (quantity < 0)
                throw new ArgumentException("Product quantity cannot be negative.", nameof(quantity));

            Id = id;
            Name = name.Trim();
            SKU = sku.Trim();
            Price = price;
            Quantity = quantity;
        }

        public void AddStock(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Stock quantity must be positive.");

            Quantity += quantity;
        }

        public void RemoveStock(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Stock quantity must be positive.");

            if (quantity > Quantity)
                throw new InvalidOperationException("Insufficient stock.");

            Quantity -= quantity;
        }
    }
}
