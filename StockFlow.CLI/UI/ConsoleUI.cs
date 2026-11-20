using StockFlow.CLI.Models;
using StockFlow.CLI.Services;

namespace StockFlow.CLI.UI
{
    public class ConsoleUI
    {
        private readonly InventoryService _inventoryService = new();

        public void Run()
        {
            Greeting();

            while (true)
            {
                Menu();

                Console.Write("\nEnter your choice: ");

                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("\nPlease enter a valid number.");
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        AddProduct();
                        break;
                    case 2:
                        ViewProducts();
                        break;
                    case 3:
                        StockIn();
                        break;
                    case 4:
                        StockOut();
                        break;
                    case 5:
                        ViewStockHistory();
                        break;
                    case 6:
                        LowStockReport();
                        break;
                    case 7:
                        SearchProducts();
                        break;
                    case 8:
                        UpdateProduct();
                        break;
                    case 9:
                        DisplayInventorySummary();
                        break;
                    case 10:
                        SortProducts();
                        break;
                    case 11:
                        DeleteProduct();
                        break;
                    case 12:
                        RestoreProduct();
                        break;
                    case 13:
                        ViewDeletedProducts();
                        break;
                    case 0:
                        Console.WriteLine("\nExiting the program.\nThank you for using StockFlow!\n");
                        return;
                    default:
                        Console.WriteLine("\nInvalid choice. Please try again.");
                        break;
                }
            }
        }

        private void Greeting()
        {

            Console.Write("\nEnter your name: ");
            string name = Console.ReadLine() ?? "";

            Console.WriteLine($"\nWelcome to StockFlow, {name}!");
        }

        private void Menu()
        {
            Console.WriteLine("\n=================================");
            Console.WriteLine("      StockFlow - Menu");
            Console.WriteLine("=================================\n");

            Console.WriteLine("1. Add Product");
            Console.WriteLine("2. View Products");
            Console.WriteLine("3. Stock In");
            Console.WriteLine("4. Stock Out");
            Console.WriteLine("5. View Stock History");
            Console.WriteLine("6. Low Stock Report");
            Console.WriteLine("7. Search Products");
            Console.WriteLine("8. Update Product");
            Console.WriteLine("9. Display Inventory Summary");
            Console.WriteLine("10. Sort Products");
            Console.WriteLine("11. Delete Product");
            Console.WriteLine("12. Restore Product");
            Console.WriteLine("13. View Deleted Products");
            Console.WriteLine("0. Exit");
            Console.WriteLine("=================================");
        }

        private void AddProduct()
        {
            Console.WriteLine("\nAdd Product");
            Console.WriteLine("------------");

            Console.Write("Enter product name: ");
            string name = Console.ReadLine()?.Trim() ?? "";

            Console.Write("Enter product SKU: ");
            string sku = ReadSku();

            Console.Write("Enter product price: ");

            if (!decimal.TryParse(Console.ReadLine(), out decimal price)
                || price <= 0)
            {
                Console.WriteLine("Invalid price. Product not added.");
                return;
            }

            Console.Write("Enter initial quantity: ");

            if (!int.TryParse(Console.ReadLine(), out int quantity)
                || quantity < 0)
            {
                Console.WriteLine("Invalid quantity. Product not added.");
                return;
            }

            _inventoryService.AddProduct(name, sku, price, quantity);
        }

        private void ViewProducts()
        {
            Console.WriteLine("\nView Products");
            Console.WriteLine("---------------");

            _inventoryService.ViewProducts();
        }

        private void StockIn()
        {
            Console.WriteLine("\nStock In");
            Console.WriteLine("----------");

            string skuIn = ReadSku();
            int quantityIn = ReadPositiveQuantity();

            _inventoryService.StockIn(skuIn, quantityIn);
        }

        private void StockOut()
        {
            Console.WriteLine("\nStock Out");
            Console.WriteLine("-----------");

            string skuOut = ReadSku();
            int quantityOut = ReadPositiveQuantity();

            _inventoryService.StockOut(skuOut, quantityOut);
        }

        private void ViewStockHistory()
        {
            Console.WriteLine("\nView Stock History");
            Console.WriteLine("-------------------");

            Console.WriteLine("1. View All Movements");
            Console.WriteLine("2. Filter by Product SKU");

            Console.Write("\nEnter your choice: ");

            if (!int.TryParse(Console.ReadLine(), out int historyChoice))
            {
                Console.WriteLine("Invalid choice.");
                return;
            }

            switch (historyChoice)
            {
                case 1:
                    _inventoryService.ViewStockHistory();
                    break;

                case 2:
                    string historySku = ReadSku();
                    _inventoryService.ViewStockHistory(historySku);

                    break;

                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }

        private void LowStockReport()
        {
            Console.WriteLine("\nLow Stock Report");
            Console.WriteLine("-----------------");

            Console.Write("Enter low stock threshold: ");

            if (int.TryParse(Console.ReadLine(), out int threshold)
                && threshold >= 0)
            {
                _inventoryService.LowStockReport(threshold);
            }
            else
            {
                Console.WriteLine("Invalid threshold. Enter a non-negative integer.");
            }
        }

        private void SearchProducts()
        {
            Console.WriteLine("Enter a search term (name or SKU): ");

            string searchTerm = Console.ReadLine()?.Trim() ?? "";

            List<Product> matchingProducts = _inventoryService.SearchProducts(searchTerm);

            if (matchingProducts.Count == 0)
            {
                Console.WriteLine($"No products found matching '{searchTerm}'.");
                return;
            }

            Console.WriteLine($"\nTotal Matching Products: {matchingProducts.Count}");
            Console.WriteLine("---------------------------");

            foreach (Product product in matchingProducts)
            {
                Console.WriteLine($"Product ID: {product.Id}");
                Console.WriteLine($"Name: {product.Name}");
                Console.WriteLine($"SKU: {product.SKU}");
                Console.WriteLine($"Price: {product.Price:C}");
                Console.WriteLine($"Quantity: {product.Quantity}");
                Console.WriteLine("---------------------------");
            }
        }

        private void UpdateProduct()
        {
            Console.WriteLine("\nUpdate Product");
            Console.WriteLine("----------------");

            Console.Write("Enter product SKU: ");
            string sku = Console.ReadLine()?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(sku))
            {
                Console.WriteLine("SKU cannot be empty.");
                return;
            }

            Product? product;

            try
            {
                product = _inventoryService.FindProductBySku(sku);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            Console.WriteLine($"\nCurrent Name: {product.Name}");
            Console.WriteLine($"Current Price: {product.Price:C}");

            Console.Write("\nEnter new name (press Enter to keep current): ");
            string nameInput = Console.ReadLine()?.Trim() ?? "";

            string newName = string.IsNullOrWhiteSpace(nameInput)
                ? product.Name
                : nameInput;

            Console.Write("Enter new price (press Enter to keep current): ");
            string priceInput = Console.ReadLine()?.Trim() ?? "";

            decimal newPrice = product.Price;

            if (!string.IsNullOrWhiteSpace(priceInput))
            {
                if (!decimal.TryParse(priceInput, out newPrice) || newPrice <= 0)
                {
                    Console.WriteLine("Invalid price. Update aborted.");
                    return;
                }
            }

            _inventoryService.UpdateProduct(sku, newName, newPrice);
        }

        private void DisplayInventorySummary()
        {
            Console.WriteLine("\nInventory Summary");
            Console.WriteLine("------------------");

            _inventoryService.DisplayInventorySummary();
        }

        private void SortProducts()
        {
            Console.WriteLine("\nSort Products");
            Console.WriteLine("--------------");

            Console.WriteLine("Select a sorting option:");
            Console.WriteLine("1. Sort by Name (A-Z)");
            Console.WriteLine("2. Sort by Name (Z-A)");
            Console.WriteLine("3. Sort by Price (Low to High)");
            Console.WriteLine("4. Sort by Price (High to Low)");
            Console.WriteLine("5. Sort by Quantity (Low to High)");
            Console.WriteLine("6. Sort by Quantity (High to Low)");

            Console.Write("\nEnter your choice: ");

            if (!int.TryParse(Console.ReadLine(), out int sortOption) ||
                sortOption < 1 || sortOption > 6)
            {
                Console.WriteLine("Invalid sorting option. Please select 1-6.");
                return;
            }

            List<Product> sortedProducts = _inventoryService.SortProducts(sortOption);

            if (sortedProducts.Count == 0)
            {
                Console.WriteLine("No products available to sort.");
                return;
            }

            Console.WriteLine("\nSorted Products");
            Console.WriteLine("----------------");

            foreach (Product product in sortedProducts)
            {
                Console.WriteLine($"Product ID: {product.Id}");
                Console.WriteLine($"Name: {product.Name}");
                Console.WriteLine($"SKU: {product.SKU}");
                Console.WriteLine($"Price: {product.Price:C}");
                Console.WriteLine($"Quantity: {product.Quantity}");
                Console.WriteLine("---------------------------");
            }
        }

        private void DeleteProduct()
        {
            Console.WriteLine("\nDelete Product");
            Console.WriteLine("----------------");

            Console.Write("Enter product SKU to delete: ");
            string sku = Console.ReadLine()?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(sku))
            {
                Console.WriteLine("SKU cannot be empty.");
                return;
            }

            try
            {
                Product product = _inventoryService.FindProductBySku(sku);

                Console.WriteLine("\nProduct Details");
                Console.WriteLine("----------------");
                Console.WriteLine($"Name: {product.Name}");
                Console.WriteLine($"SKU: {product.SKU}");
                Console.WriteLine($"Quantity: {product.Quantity}");

                Console.Write("\nAre you sure you want to delete this product? (Y/N): ");
                string confirmation = Console.ReadLine()?.Trim() ?? "";

                if (!confirmation.Equals("Y", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Product deletion cancelled.");
                    return;
                }

                _inventoryService.DeleteProduct(sku);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void ViewDeletedProducts()
        {
            Console.WriteLine("\nDeleted Products");
            Console.WriteLine("-----------------");

            List<Product> deletedProducts = _inventoryService.GetDeletedProducts();

            if (deletedProducts.Count == 0)
            {
                Console.WriteLine("No deleted products found.");
                return;
            }

            Console.WriteLine($"Total Deleted Products: {deletedProducts.Count}");

            foreach (Product product in deletedProducts)
            {
                Console.WriteLine($"Product ID: {product.Id}");
                Console.WriteLine($"Name: {product.Name}");
                Console.WriteLine($"SKU: {product.SKU}");
                Console.WriteLine($"Price: {product.Price:C}");
                Console.WriteLine($"Quantity: {product.Quantity}");
                Console.WriteLine("---------------------------");
            }
        }

        private void RestoreProduct()
        {
            Console.WriteLine("\nRestore Product");
            Console.WriteLine("----------------");

            Console.Write("Enter product SKU to restore: ");
            string sku = Console.ReadLine()?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(sku))
            {
                Console.WriteLine("SKU cannot be empty.");
                return;
            }

            try
            {
                Product product = _inventoryService.FindProductIncludingDeletedBySku(sku);

                if (!product.IsDeleted)
                {
                    Console.WriteLine("This product is already active.");
                    return;
                }

                Console.WriteLine("\nProduct Details");
                Console.WriteLine("----------------");
                Console.WriteLine($"Name: {product.Name}");
                Console.WriteLine($"SKU: {product.SKU}");
                Console.WriteLine($"Quantity: {product.Quantity}");

                Console.Write("\nAre you sure you want to restore this product? (Y/N): ");
                string confirmation = Console.ReadLine()?.Trim() ?? "";

                if (!confirmation.Equals("Y", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Product restoration cancelled.");
                    return;
                }

                _inventoryService.RestoreProduct(sku);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static string ReadSku()
        {
            while (true)
            {
                Console.Write("Enter product SKU: ");

                string? sku = Console.ReadLine()?.Trim();

                if (!string.IsNullOrWhiteSpace(sku))
                {
                    return sku;
                }

                Console.WriteLine("Product SKU is required.");
            }
        }

        private static int ReadPositiveQuantity()
        {
            while (true)
            {
                Console.Write("Enter stock quantity: ");

                if (int.TryParse(Console.ReadLine(), out int quantity)
                    && quantity > 0)
                {
                    return quantity;
                }

                Console.WriteLine("Please enter a positive integer.");
            }
        }

    }
}
