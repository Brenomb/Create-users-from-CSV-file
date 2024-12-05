using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;

namespace Create_users_from_CSV_file
{
    public class ReadCSV
    {
        public List<UserColumns> ReadCSVFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException("File path cannot be null or empty.");
            }

            var users = new List<UserColumns>();

            try
            {
                using (TextFieldParser parser = new TextFieldParser(filePath))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(";");

                    // Skip the header row
                    string[] headers = parser.ReadFields();

                    while (!parser.EndOfData)
                    {
                        string[] fields = parser.ReadFields();

                        // Map fields to UserColumns
                        var user = new UserColumns
                        {
                            UserId = fields[0],
                            Login = fields[1],
                            FirstName = fields[2],
                            LastName = fields[3],
                            Email = fields[4],
                            GovId = fields[5],
                            CellPhone = fields[6],
                            Address = fields[7],
                            City = fields[8],
                            PostalCode = fields[9],
                            Country = fields[10],
                            Province = fields[11],
                            Region = fields[12]
                        };

                        users.Add(user);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while reading the CSV file: {ex.Message}");
            }

            return users;
        }
    }
}
