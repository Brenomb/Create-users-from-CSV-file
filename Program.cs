using System;

namespace Create_users_from_CSV_file;
class Program
{
    static void Main(string[] args)
    {
        PythonIntegration pythonIntegration = new PythonIntegration();

        string pythonPath = Environment.GetEnvironmentVariable("PYTHON_PATH") ?? throw new InvalidOperationException("PYTHON_PATH environment variable is not set.");
        string scriptPath = Environment.GetEnvironmentVariable("SCRIPT_PATH") ?? throw new InvalidOperationException("SCRIPT_PATH environment variable is not set.");
        string inputFile = Environment.GetEnvironmentVariable("INPUT_FILE") ?? throw new InvalidOperationException("INPUT_FILE environment variable is not set.");
        string outputFile = Environment.GetEnvironmentVariable("OUTPUT_FILE") ?? throw new InvalidOperationException("OUTPUT_FILE environment variable is not set.");

        // Desired columns in order
        string[] desiredColumns = { "ID_Utente", "Login", "Nome", "Cognome", "E-mail", "Codice_Fiscale", "Cellulare", "Indirizzo", "Città", "Codice_Postale", "Paese", "Provincia", "Regione" };
        //string[] desiredColumns = { "ID Utente", "Login", "Nome", "Cognome", "E-mail", "Codice Fiscale", "Cellulare", "Indirizzo", "Città", "Codice Postale", "Paese", "Provincia", "Regione" };

        pythonIntegration.RunPythonScript(pythonPath, scriptPath, inputFile, outputFile, desiredColumns);
    }
}