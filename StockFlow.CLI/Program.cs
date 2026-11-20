using StockFlow.CLI.UI;

Console.WriteLine("=================================");
Console.WriteLine("          STOCKFLOW");
Console.WriteLine("   Inventory Management System");
Console.WriteLine("=================================");

try
{
    ConsoleUI consoleUI = new();

    consoleUI.Run();
}
catch (InvalidDataException ex)
{
    Console.WriteLine($"\nUnable to start StockFlow: {ex.Message}");
    Console.WriteLine("No changes were made to your inventory. Fix the data file and try again.");
    Environment.ExitCode = 1;
}
