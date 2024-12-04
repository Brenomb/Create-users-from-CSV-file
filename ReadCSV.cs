using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;

namespace Create_users_from_CSV_file
{
    public class ReadCSV
    {
        public List<UserColumns> ReadCSVFile(string inputFilePath)
        {
            if (string.IsNullOrEmpty(inputFilePath))
            {
                throw new ArgumentException("Input file path cannot be null or empty.");
            }

            var users = new List<UserColumns>();

            try
            {
                using (TextFieldParser parser = new TextFieldParser(inputFilePath))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(";");

                    // Assuming the first row contains headers
                    if (!parser.EndOfData)
                    {
                        parser.ReadLine(); // Skip header row
                    }

                    while (!parser.EndOfData)
                    {
                        string[] fields = parser.ReadFields();

                        // Map fields to UserColumns properties
                        users.Add(new UserColumns
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
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return users;
        }

        public void WriteToFile(string outputFilePath, List<UserColumns> users)
        {
            if (string.IsNullOrEmpty(outputFilePath))
            {
                throw new ArgumentException("Output file path cannot be null or empty.");
            }

            try
            {
                using (StreamWriter writer = new StreamWriter(outputFilePath, false))
                {
                    // Write header
                    writer.WriteLine("UserId;Login;FirstName;LastName;Email;GovId;CellPhone;Address;City;PostalCode;Country;Province;Region");

                    // Write user data
                    foreach (var user in users)
                    {
                        writer.WriteLine($"{user.UserId};{user.Login};{user.FirstName};{user.LastName};{user.Email};{user.GovId};{user.CellPhone};{user.Address};{user.City};{user.PostalCode};{user.Country};{user.Province};{user.Region}");
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

    }
}
