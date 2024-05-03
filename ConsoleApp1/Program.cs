using System;
using System.Data;
using System.IO;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        // Specify the path to your data file
        string inputFilePath = @"C:\Users\srate\source\repos\ConsoleApp1\ConsoleApp1\Ix_Iy.csv";

        // Read the data from the file into a DataTable
        DataTable dataTable = ReadDataIntoDataTable(inputFilePath);

        // Add a new column to store the result of multiplication
        dataTable.Columns.Add("Product_of_Columns_Ix_and_Iy", typeof(double));

        // Perform multiplication and store the result in the new column using LINQ
        foreach (DataRow row in dataTable.AsEnumerable())
        {
            double product = row.ItemArray
                                .OfType<string>()
                                .Select(x => double.TryParse(x.Replace(',', '.'), out double value) ? value : 1)
                                .Aggregate((x, y) => x * y);
            row["Product_of_Columns_Ix_and_Iy"] = product;
        }

        // Prepare the path for the output file in the same directory with a different name
        string outputFilePath = Path.Combine(Path.GetDirectoryName(inputFilePath), "processed_data_with_product.csv");

        // Save the updated DataTable to a new CSV file with a different name
        SaveDataTableToCsv(dataTable, outputFilePath);

        Console.WriteLine($"Processed data with product saved to: {outputFilePath}");
        Console.ReadLine(); // Keep the console window open
    }

    static DataTable ReadDataIntoDataTable(string filePath)
    {
        DataTable dataTable = new DataTable();

        try
        {
            // Open the file and read its contents into a DataTable
            using (StreamReader sr = new StreamReader(filePath))
            {
                string line;
                bool headerRow = true;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] dataValues = line.Split(';');

                    if (headerRow)
                    {
                        // Create columns based on the header row
                        foreach (var header in dataValues)
                        {
                            dataTable.Columns.Add(header);
                        }
                        headerRow = false;
                    }
                    else
                    {
                        // Add data rows
                        dataTable.Rows.Add(dataValues);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while reading the file: {ex.Message}");
        }

        return dataTable;
    }

    static void SaveDataTableToCsv(DataTable dataTable, string filePath)
    {
        try
        {
            // Write the DataTable to a CSV file
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                // Write the header row
                sw.WriteLine(string.Join(";", dataTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName)));

                // Write the data rows
                foreach (DataRow row in dataTable.Rows)
                {
                    sw.WriteLine(string.Join(";", row.ItemArray.Select(i => i.ToString().Replace(',', '.'))));
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while writing to the file: {ex.Message}");
        }
    }
}
