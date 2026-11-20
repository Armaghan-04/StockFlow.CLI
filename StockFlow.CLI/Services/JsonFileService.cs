using System.Text.Json;
using StockFlow.CLI.Models;

namespace StockFlow.CLI.Services
{
    public class JsonFileService
    {
        private const string ProjectFileName = "StockFlow.CLI.csproj";

        private readonly string _dataDirectory;
        private readonly string _productsFilePath;
        private readonly string _stockMovementsFilePath;

        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true
        };

        public JsonFileService()
        {
            _dataDirectory = Path.Combine(FindProjectRoot(), "Data");

            _productsFilePath = Path.Combine(_dataDirectory, "products.json");

            _stockMovementsFilePath = Path.Combine(_dataDirectory, "stock-movements.json");

            try
            {
                Directory.CreateDirectory(_dataDirectory);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                throw new InvalidDataException(
                    $"Could not create the data directory '{_dataDirectory}': {ex.Message}", ex);
            }
        }

        public void SaveProducts(List<Product> products)
        {
            string json = JsonSerializer.Serialize(products, _jsonOptions);

            WriteFile(_productsFilePath, json, "products");
        }

        public List<Product> LoadProducts()
        {
            return LoadList(_productsFilePath, "products", json =>
                JsonSerializer.Deserialize<List<Product>>(json, _jsonOptions));
        }

        public void SaveStockMovements(List<StockMovement> movements)
        {
            string json = JsonSerializer.Serialize(movements, _jsonOptions);

            WriteFile(_stockMovementsFilePath, json, "stock movements");
        }

        public List<StockMovement> LoadStockMovements()
        {
            return LoadList(_stockMovementsFilePath, "stock movements", json =>
                JsonSerializer.Deserialize<List<StockMovement>>(json, _jsonOptions));
        }

        private static string FindProjectRoot()
        {
            DirectoryInfo? directory = new(AppContext.BaseDirectory);

            while (directory is not null)
            {
                if (File.Exists(Path.Combine(directory.FullName, ProjectFileName)))
                    return directory.FullName;

                directory = directory.Parent;
            }

            // Published builds have no .csproj, so keep data beside the executable.
            return AppContext.BaseDirectory;
        }

        private void WriteFile(string filePath, string json, string description)
        {
            try
            {
                File.WriteAllText(filePath, json);
            }
            catch (IOException ex)
            {
                throw new IOException(
                    $"Could not save {description} to '{filePath}': {ex.Message}. " +
                    "Check that the file is not locked by another program.", ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new UnauthorizedAccessException(
                    $"Access denied while saving {description} to '{filePath}': {ex.Message}. " +
                    "Check the file permissions.", ex);
            }
        }

        private static List<T> LoadList<T>(
            string filePath,
            string description,
            Func<string, List<T>?> deserialize)
        {
            if (!File.Exists(filePath))
                return [];

            string json;

            try
            {
                json = File.ReadAllText(filePath);
            }
            catch (IOException ex)
            {
                throw new InvalidDataException(
                    $"Could not read {description} from '{filePath}': {ex.Message}. " +
                    "Check that the file is not locked by another program.", ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new InvalidDataException(
                    $"Access denied while reading {description} from '{filePath}': {ex.Message}. " +
                    "Check the file permissions.", ex);
            }

            try
            {
                return deserialize(json)
                    ?? throw new InvalidDataException(
                        $"The {description} file '{filePath}' contains no data.");
            }
            catch (JsonException ex)
            {
                throw new InvalidDataException(
                    $"The {description} file '{filePath}' contains invalid JSON: {ex.Message}. " +
                    "Fix the file or remove it to start with an empty inventory.", ex);
            }
            catch (ArgumentException ex)
            {
                throw new InvalidDataException(
                    $"The {description} file '{filePath}' contains invalid data: {ex.Message}. " +
                    "Fix the file or remove it to start with an empty inventory.", ex);
            }
        }
    }
}
