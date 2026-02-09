using System;
using System.IO;
using System.Linq;
using System.Globalization;

/// <summary>
/// Data model representing a single sales item from the CSV file.
/// Contains product name, price, and quantity information.
/// </summary>
public record SalesItem(string ProductName, decimal Price, int Quantity);

/// <summary>
/// SalesAnalyzer class for analyzing sales data from CSV files.
/// Provides functionality to load sales data, calculate total sales, and identify top-performing products.
/// </summary>
public class SalesAnalyzer
{
    private readonly string _filename;
    private List<SalesItem> _sales = new List<SalesItem>();

    /// <summary>
    /// Initializes a new instance of the SalesAnalyzer class with the specified filename.
    /// </summary>
    /// <param name="filename">The path to the CSV file containing sales data.</param>
    public SalesAnalyzer(string filename)
    {
        _filename = filename;
    }

    /// <summary>
    /// Loads sales data from the CSV file into memory.
    /// Expects the CSV format: product_name,price,quantity
    /// Skips the header row and validates each data row before loading.
    /// </summary>
    /// <returns>True if data was loaded successfully; false otherwise.</returns>
    public bool LoadData()
    {
        try
        {
            if (!File.Exists(_filename))
            {
                Console.WriteLine($"Error: The file '{_filename}' was not found.");
                return false;
            }

            _sales.Clear();
            var dataLines = File.ReadLines(_filename).Skip(1);

            foreach (string line in dataLines)
            {
                string[] columns = line.Split(',');

                if (columns.Length == 3)
                {
                    if (decimal.TryParse(columns[1], NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price) &&
                        int.TryParse(columns[2], out int quantity))
                    {
                        _sales.Add(new SalesItem(columns[0].Trim(), price, quantity));
                    }
                    else
                    {
                        Console.WriteLine($"Skipping malformed data in row: {line}");
                    }
                }
            }

            return true;
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine($"Error: The file '{_filename}' was not found.");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred while loading data: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Calculates the total sales revenue from all loaded sales items.
    /// Multiplies each product's price by its quantity and sums the results.
    /// </summary>
    /// <returns>The total sales revenue as a decimal value.</returns>
    public decimal CalculateTotalSales()
    {
        try
        {
            decimal total = 0.0m;

            foreach (var item in _sales)
            {
                total += item.Price * item.Quantity;
            }

            return total;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred while calculating total sales: {ex.Message}");
            return 0.0m;
        }
    }

    /// <summary>
    /// Finds the top-selling product based on total revenue (price × quantity).
    /// </summary>
    /// <returns>A tuple containing the product name and its total revenue. 
    /// Returns ("No valid data", 0.0m) if no products are found.</returns>
    public (string ProductName, decimal Revenue) FindTopProduct()
    {
        try
        {
            if (_sales.Count == 0)
            {
                return ("No valid data", 0.0m);
            }

            var topProduct = _sales
                .Select(item => new
                {
                    item.ProductName,
                    Revenue = item.Price * item.Quantity
                })
                .OrderByDescending(product => product.Revenue)
                .First();

            return (topProduct.ProductName, topProduct.Revenue);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred while finding top product: {ex.Message}");
            return ("Error", 0.0m);
        }
    }
}

/// <summary>
/// Main program entry point. Demonstrates usage of the SalesAnalyzer class.
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        string salesDataFile = "sales_data.csv";
        
        // Create a dummy file if it doesn't exist for testing
        if (!File.Exists(salesDataFile))
        {
            File.WriteAllText(salesDataFile, "product_name,price,quantity\nLaptop,1200.00,5\nMouse,25.50,10\n");
            Console.WriteLine($"Created sample file: {salesDataFile}\n");
        }

        // Instantiate the SalesAnalyzer with the filename
        SalesAnalyzer analyzer = new SalesAnalyzer(salesDataFile);

        // Load the data from the CSV file
        if (!analyzer.LoadData())
        {
            Console.WriteLine("Failed to load sales data. Exiting.");
            return;
        }

        // Calculate and display total sales
        decimal totalSales = analyzer.CalculateTotalSales();
        Console.WriteLine($"Total sales from {salesDataFile}: {totalSales.ToString("C", CultureInfo.CurrentCulture)}");

        // Find and display the top-selling product
        var (topProduct, revenue) = analyzer.FindTopProduct();
        Console.WriteLine($"Top-Selling Product: {topProduct} with Revenue: {revenue:C}");
    }
}
