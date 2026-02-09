using System;
using System.IO;
using System.Linq;
using System.Globalization;

/// <summary>
/// A class to analyze sales data from a CSV file.
/// </summary>
public class SalesAnalyzer
{
    private readonly string _filename;

    /// <summary>
    /// Initializes the SalesAnalyzer with the specified filename.
    /// </summary>
    /// <param name="filename">The path to the CSV file containing sales data.</param>
    public SalesAnalyzer(string filename)
    {
        _filename = filename;
    }

    /// <summary>
    /// Loads and validates the sales data from the file.
    /// </summary>
    public void LoadData()
    {
        if (!File.Exists(_filename))
        {
            throw new FileNotFoundException($"The file '{_filename}' does not exist.");
        }
    }

    /// <summary>
    /// Calculates the total sales from the data.
    /// </summary>
    /// <returns>The total sales as a decimal value.</returns>
    public decimal CalculateTotalSales()
    {
        decimal total = 0.0m;
        try
        {
            var dataLines = File.ReadLines(_filename).Skip(1);

            foreach (string line in dataLines)
            {
                string[] columns = line.Split(',');

                if (columns.Length == 3 &&
                    decimal.TryParse(columns[1], NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price) &&
                    int.TryParse(columns[2], out int quantity))
                {
                    total += price * quantity;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while calculating total sales: {ex.Message}");
        }

        return total;
    }

    /// <summary>
    /// Finds the top-selling product by revenue.
    /// </summary>
    /// <returns>A tuple containing the product name and its revenue.</returns>
    public (string ProductName, decimal Revenue) FindTopProduct()
    {
        try
        {
            var dataLines = File.ReadLines(_filename).Skip(1);

            var topProduct = dataLines
                .Select(line => line.Split(','))
                .Where(columns => columns.Length == 3 &&
                    decimal.TryParse(columns[1], NumberStyles.Any, CultureInfo.InvariantCulture, out _) &&
                    int.TryParse(columns[2], out _))
                .Select(columns => new
                {
                    ProductName = columns[0],
                    Revenue = decimal.Parse(columns[1], NumberStyles.Any, CultureInfo.InvariantCulture) * int.Parse(columns[2])
                })
                .OrderByDescending(product => product.Revenue)
                .FirstOrDefault();

            if (topProduct != null)
            {
                return (topProduct.ProductName, topProduct.Revenue);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while finding the top product: {ex.Message}");
        }

        return ("No valid data", 0.0m);
    }
}

/// <summary>
/// Main execution block to demonstrate the functionality of the SalesAnalyzer class.
/// </summary>
public class Program
{
    public static void Main(string[] args)
    {
        string salesDataFile = "sales_data.csv";

        if (!File.Exists(salesDataFile))
        {
            File.WriteAllText(salesDataFile, "product_name,price,quantity\nLaptop,1200.00,5\nMouse,25.50,10\n");
        }

        SalesAnalyzer analyzer = new SalesAnalyzer(salesDataFile);
        analyzer.LoadData();

        decimal totalSales = analyzer.CalculateTotalSales();
        Console.WriteLine($"Total sales: {totalSales:C}");

        var (topProduct, revenue) = analyzer.FindTopProduct();
        Console.WriteLine($"Top-Selling Product: {topProduct} with Revenue: {revenue:C}");
    }
}