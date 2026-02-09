import csv


class SalesAnalyzer:
    """
    A class to analyze sales data from a CSV file.
    
    This class provides methods to load sales data, calculate total sales,
    and identify the top-selling product by revenue.
    """
    
    def __init__(self, filename):
        """
        Initialize the SalesAnalyzer with a CSV filename.
        
        Args:
            filename (str): The path to the CSV file containing sales data.
                           Expected columns: 'product_name', 'price', 'quantity'
        """
        self.filename = filename
        self.data = []
    
    def load_data(self):
        """
        Load sales data from the CSV file.
        
        Reads the CSV file and stores all rows as dictionaries in self.data.
        Handles file not found and other exceptions gracefully.
        
        Returns:
            bool: True if data loaded successfully, False otherwise.
        """
        try:
            with open(self.filename, mode='r') as file:
                csv_reader = csv.DictReader(file)
                self.data = list(csv_reader)
            return True
        except FileNotFoundError:
            print(f"Error: The file '{self.filename}' was not found.")
            return False
        except Exception as e:
            print(f"An unexpected error occurred while loading data: {e}")
            return False
    
    def calculate_total_sales(self):
        """
        Calculate the total sales from all products in the loaded data.
        
        Computes the total revenue by multiplying price and quantity for each row
        and summing the results. Skips rows with invalid data and reports errors.
        
        Returns:
            float: The total sales amount in dollars.
        """
        total = 0.0
        
        for row in self.data:
            try:
                price = float(row['price'])
                quantity = int(row['quantity'])
                total += price * quantity
            except ValueError:
                print(f"Error: Invalid data in row {row}. Ensure 'price' and 'quantity' are valid numbers.")
            except KeyError as e:
                print(f"Error: Missing expected column {e} in row {row}.")
        
        return total
    
    def find_top_product(self):
        """
        Identify the top-selling product by total revenue.
        
        Finds the product with the highest total revenue (price * quantity).
        
        Returns:
            tuple: A tuple containing (product_name, max_revenue).
                   Returns (None, 0.0) if no valid products are found.
        """
        top_product = None
        max_revenue = 0.0
        
        for row in self.data:
            try:
                price = float(row['price'])
                quantity = int(row['quantity'])
                revenue = price * quantity
                
                if revenue > max_revenue:
                    max_revenue = revenue
                    top_product = row['product_name']
            except ValueError:
                print(f"Error: Invalid data in row {row}. Ensure 'price' and 'quantity' are valid numbers.")
            except KeyError as e:
                print(f"Error: Missing expected column {e} in row {row}.")
        
        return top_product, max_revenue


if __name__ == "__main__":
    sales_data_file = 'sales_data.csv'
    
    # Instantiate the analyzer
    analyzer = SalesAnalyzer(sales_data_file)
    
    # Load data from the CSV file
    if analyzer.load_data():
        # Calculate and display total sales
        total_sales = analyzer.calculate_total_sales()
        print(f"Total sales from {sales_data_file}: ${total_sales:.2f}")
        
        # Find and display top-selling product
        top_product, max_revenue = analyzer.find_top_product()
        if top_product:
            print(f"Top-selling product: {top_product} with total revenue of ${max_revenue:.2f}")
        else:
            print("No valid top-selling product found.")
    else:
        print("Failed to load sales data. Exiting.")
