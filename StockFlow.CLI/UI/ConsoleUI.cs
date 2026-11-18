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
            Console.WriteLine("0. Exit");
        }

        private void AddProduct()
        {
            Console.WriteLine("\nAdd Product");
            Console.WriteLine("------------");

            Console.Write("Enter product name: ");
            string name = Console.ReadLine()?.Trim() ?? "";

            Console.Write("Enter product SKU: ");
            string sku = ReadSku() ;

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
