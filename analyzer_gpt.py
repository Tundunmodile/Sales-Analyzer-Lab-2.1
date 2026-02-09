import csv

class SalesAnalyzer:
    """
    A class to analyze sales data from a CSV file.

    Attributes:
        filename (str): The name of the CSV file containing sales data.
    """

    def __init__(self, filename):
        """
        Initializes the SalesAnalyzer with the given filename.

        Args:
            filename (str): The name of the CSV file containing sales data.
        """
        self.filename = filename

    def load_data(self):
        """
        Loads the sales data from the CSV file.

        Returns:
            list: A list of dictionaries representing rows in the CSV file.
        """
        try:
            with open(self.filename, mode='r') as file:
                csv_reader = csv.DictReader(file)
                return list(csv_reader)
        except FileNotFoundError:
            print(f"Error: The file '{self.filename}' was not found.")
        except Exception as e:
            print(f"An unexpected error occurred: {e}")
        return []

    def calculate_total_sales(self):
        """
        Calculates the total sales from the CSV file.

        Returns:
            float: The total sales amount.
        """
        total = 0.0
        data = self.load_data()

        for row in data:
            try:
                price = float(row['price'])
                quantity = int(row['quantity'])
                total += price * quantity
            except ValueError:
                print(f"Error: Invalid data in row {row}. Ensure 'price' and 'quantity' are valid numbers.")

        return total

    def find_top_product(self):
        """
        Finds the top-selling product by total revenue (price * quantity).

        Returns:
            tuple: A tuple containing the name of the top product and its total revenue.
        """
        top_product = None
        max_revenue = 0.0
        data = self.load_data()

        for row in data:
            try:
                price = float(row['price'])
                quantity = int(row['quantity'])
                revenue = price * quantity

                if revenue > max_revenue:
                    max_revenue = revenue
                    top_product = row['product_name']
            except ValueError:
                print(f"Error: Invalid data in row {row}. Ensure 'price' and 'quantity' are valid numbers.")

        return top_product, max_revenue

if __name__ == "__main__":
    sales_data_file = 'sales_data.csv'
    analyzer = SalesAnalyzer(sales_data_file)

    total_sales = analyzer.calculate_total_sales()
    print(f"Total sales from {sales_data_file}: ${total_sales:.2f}")

    top_product, max_revenue = analyzer.find_top_product()
    if top_product:
        print(f"Top-selling product: {top_product} with total revenue of ${max_revenue:.2f}")
    else:
        print("No valid top-selling product found.")