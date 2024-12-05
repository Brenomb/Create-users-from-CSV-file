using System;

namespace Create_users_from_CSV_file;
class Program
{
    static void Main(string[] args)
    {
        try
        {
            // Step 1: Run Python Script to Clean the CSV
            PythonIntegration pythonIntegration = new PythonIntegration();

            string pythonPath = Environment.GetEnvironmentVariable("PYTHON_PATH") ?? throw new InvalidOperationException("PYTHON_PATH environment variable is not set.");
            string scriptPath = Environment.GetEnvironmentVariable("SCRIPT_PATH") ?? throw new InvalidOperationException("SCRIPT_PATH environment variable is not set.");
            string inputFile = Environment.GetEnvironmentVariable("INPUT_FILE") ?? throw new InvalidOperationException("INPUT_FILE environment variable is not set.");
            string outputFile = Environment.GetEnvironmentVariable("OUTPUT_FILE") ?? throw new InvalidOperationException("OUTPUT_FILE environment variable is not set.");

            // Desired columns in order
            string[] desiredColumns = { "ID_Utente", "Login", "Nome", "Cognome", "E-mail", "Codice_Fiscale", "Cellulare", "Indirizzo", "Città", "Codice_Postale", "Paese", "Provincia", "Regione" };

            Console.WriteLine("Running Python script to clean CSV...");
            pythonIntegration.RunPythonScript(pythonPath, scriptPath, inputFile, outputFile, desiredColumns);
            Console.WriteLine("Python script completed.");

            // Step 2: Read the Cleaned CSV File
            ReadCSV csvReader = new ReadCSV();
            Console.WriteLine("Reading cleaned CSV...");
            List<UserColumns> users = csvReader.ReadCSVFile(outputFile);

            // Step 3: Insert Data into CRM
            InsertIntoCRM crmInserter = new InsertIntoCRM();
            Console.WriteLine("Inserting users into CRM...");
            crmInserter.InsertUsersIntoCRM(users);

            Console.WriteLine("Process completed successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}