using StockFlow.CLI.Enums;

namespace StockFlow.CLI.Models
{
    public class StockMovement
    {
        public Guid Id { get; private set; } = Guid.NewGuid();

        public Guid ProductId { get; private set; }

        public string SKU { get; private set; } = "";

        public MovementType MovementType { get; private set; }

        public int Quantity { get; private set; }

        public DateTime Timestamp { get; private set; } = DateTime.UtcNow;

        public StockMovement(Guid productId, string sku, MovementType movementType, int quantity)
        {
            if (productId == Guid.Empty)
                throw new ArgumentException("Product ID is required.", nameof(productId));

            if (string.IsNullOrWhiteSpace(sku))
                throw new ArgumentException("Product SKU is required.", nameof(sku));

            if (!Enum.IsDefined(movementType))
                throw new ArgumentException("Invalid movement type.", nameof(movementType));

            if (quantity <= 0)
                throw new ArgumentException("Stock movement quantity must be positive.", nameof(quantity));

            ProductId = productId;
            SKU = sku.Trim();
            MovementType = movementType;
            Quantity = quantity;
        }
    }
}
