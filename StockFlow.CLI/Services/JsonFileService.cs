using System.Text.Json;
using StockFlow.CLI.Models;

namespace StockFlow.CLI.Services
{
    public class JsonFileService
    {
        private readonly string _dataDirectory;
        private readonly string _productsFilePath;
        private readonly string _stockMovementsFilePath;

        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true
        };

        public JsonFileService()
        {
            _dataDirectory = Path.Combine(AppContext.BaseDirectory, "Data");

            _productsFilePath = Path.Combine(_dataDirectory, "products.json");

            _stockMovementsFilePath = Path.Combine(_dataDirectory, "stock-movements.json");

            Directory.CreateDirectory(_dataDirectory);
        }

        public void SaveProducts(List<Product> products)
        {
            string json = JsonSerializer.Serialize(products, _jsonOptions);

            File.WriteAllText(_productsFilePath, json);
        }

        public List<Product> LoadProducts()
        {
            if (!File.Exists(_productsFilePath))
                return [];

            try
            {
                string json = File.ReadAllText(_productsFilePath);

                return JsonSerializer.Deserialize<List<Product>>(json, _jsonOptions) ?? [];

            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error loading JSON data: {ex.Message}");
                return [];
            }
            catch (IOException ex)
            {
                Console.WriteLine($"File access error: {ex.Message}");
                return [];
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Permission error: {ex.Message}");
                return [];
            }
        }

        public void SaveStockMovements(List<StockMovement> movements)
        {
            string json = JsonSerializer.Serialize(movements, _jsonOptions);

            File.WriteAllText(_stockMovementsFilePath, json);
        }

        public List<StockMovement> LoadStockMovements()
        {
            if (!File.Exists(_stockMovementsFilePath))
                return [];

            try
            {
                string json = File.ReadAllText(_stockMovementsFilePath);

                return JsonSerializer.Deserialize<List<StockMovement>>(json, _jsonOptions) ?? [];
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error loading JSON data: {ex.Message}");
                return [];
            }
            catch (IOException ex)
            {
                Console.WriteLine($"File access error: {ex.Message}");
                return [];
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Permission error: {ex.Message}");
                return [];
            }
        }
    }
}
