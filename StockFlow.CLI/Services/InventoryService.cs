using StockFlow.CLI.Models;
using StockFlow.CLI.Enums;

namespace StockFlow.CLI.Services
{
    public class InventoryService
    {
        private readonly List<Product> _products = [];

        private readonly List<StockMovement> _stockMovements = [];

        private readonly JsonFileService _jsonFileService = new();

        public InventoryService()
        {
            _products.AddRange(_jsonFileService.LoadProducts());

            _stockMovements.AddRange(_jsonFileService.LoadStockMovements());
        }

        public void AddProduct(string name, string sku, decimal price, int quantity)
        {
            try
            {
                if (_products.Any(p => string.Equals(
                    p.SKU, sku, StringComparison.OrdinalIgnoreCase)))
                {
                    Console.WriteLine("A product with this SKU already exists.");
                    return;
                }

                Product product = new(name, sku, price, quantity);

                _products.Add(product);

                if (quantity > 0)
                    _stockMovements
                        .Add(new StockMovement(product.Id, product.SKU, MovementType.In, quantity));

                _jsonFileService.SaveProducts(_products);
                _jsonFileService.SaveStockMovements(_stockMovements);

                Console.WriteLine($"\nProduct '{product.Name}' added successfully!");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error saving inventory data: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Permission error while saving data: {ex.Message}");
            }
        }

        public void ViewProducts()
        {
            if (_products.Count == 0)
            {
                Console.WriteLine("No products available.");
                return;
            }

            Console.WriteLine($"Total Products: {_products.Count}");

            foreach (Product product in _products)
            {
                Console.WriteLine($"Product ID: {product.Id}");
                Console.WriteLine($"Name: {product.Name}");
                Console.WriteLine($"SKU: {product.SKU}");
                Console.WriteLine($"Price: {product.Price:C}");
                Console.WriteLine($"Quantity: {product.Quantity}");
                Console.WriteLine("---------------------------");
            }
        }

        public void StockIn(string sku, int quantity)
        {
            try
            {
                Product product = FindProductBySku(sku);

                product.AddStock(quantity);

                _stockMovements
                    .Add(new StockMovement(product.Id, product.SKU, MovementType.In, quantity));

                _jsonFileService.SaveProducts(_products);
                _jsonFileService.SaveStockMovements(_stockMovements);

                Console.WriteLine($"Stock added successfully. New quantity: {product.Quantity}");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error saving inventory data: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Permission error while saving data: {ex.Message}");
            }
        }

        public void StockOut(string sku, int quantity)
        {
            try
            {
                Product product = FindProductBySku(sku);

                product.RemoveStock(quantity);

                _stockMovements
                    .Add(new StockMovement(product.Id, product.SKU, MovementType.Out, quantity));

                _jsonFileService.SaveProducts(_products);
                _jsonFileService.SaveStockMovements(_stockMovements);

                Console.WriteLine($"Stock removed successfully.\nNew quantity: {product.Quantity}");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error saving inventory data: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Permission error while saving data: {ex.Message}");
            }
        }

        public void ViewStockHistory(string? sku = null)
        {
            if (_stockMovements.Count == 0)
            {
                Console.WriteLine("No stock movements recorded yet.");
                return;
            }

            IEnumerable<StockMovement> movements = _stockMovements;

            if (!string.IsNullOrWhiteSpace(sku))
            {
                movements = movements.Where(m =>
                    string.Equals(m.SKU, sku, StringComparison.OrdinalIgnoreCase));
            }

            List<StockMovement> orderedMovements = [.. movements
                .OrderByDescending(m => m.Timestamp)];

            if (orderedMovements.Count == 0)
            {
                Console.WriteLine($"No stock movements found for SKU '{sku}'.");
                return;
            }

            if (string.IsNullOrWhiteSpace(sku))
            {
                Console.WriteLine($"Total Stock Movements: {orderedMovements.Count}");
            }
            else
            {
                Console.WriteLine($"Total Stock Movements for SKU '{sku}': {orderedMovements.Count}");
            }

            foreach (StockMovement movement in orderedMovements)
            {
                Console.WriteLine($"SKU: {movement.SKU}");
                Console.WriteLine($"Movement Type: {movement.MovementType}");
                Console.WriteLine($"Quantity: {movement.Quantity}");
                Console.WriteLine($"Timestamp: {movement.Timestamp.ToLocalTime()}");
                Console.WriteLine("---------------------------");
            }
        }

        public void LowStockReport(int threshold)
        {
            if (_products.Count == 0)
            {
                Console.WriteLine("No products available.");
                return;
            }

            if (threshold < 0)
            {
                Console.WriteLine("Threshold cannot be negative.");
                return;
            }

            List<Product> lowStockProducts = [.. _products
                .Where(p => p.Quantity <= threshold)];

            if (lowStockProducts.Count == 0)
            {
                Console.WriteLine("No low-stock products found.");
                return;
            }

            Console.WriteLine("\n=================================");
            Console.WriteLine("         LOW STOCK REPORT");
            Console.WriteLine("=================================");
            Console.WriteLine($"Threshold: {threshold}");
            Console.WriteLine($"Total Low Stock Products: {lowStockProducts.Count}");
            Console.WriteLine("---------------------------");

            foreach (Product product in lowStockProducts)
            {
                Console.WriteLine($"Product ID: {product.Id}");
                Console.WriteLine($"Name: {product.Name}");
                Console.WriteLine($"SKU: {product.SKU}");
                Console.WriteLine($"Price: {product.Price:C}");
                Console.WriteLine($"Quantity: {product.Quantity}");
                Console.WriteLine("---------------------------");
            }
        }

        private Product FindProductBySku(string sku)
        {
            return _products.FirstOrDefault(p =>
                string.Equals(p.SKU, sku, StringComparison.OrdinalIgnoreCase))
                ?? throw new InvalidOperationException("Product not found.");
        }
    }
}
