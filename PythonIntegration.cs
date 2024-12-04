using System;
using System.Diagnostics;

namespace Create_users_from_CSV_file
{
    public class PythonIntegration
    {
        public void RunPythonScript(string pythonPath, string scriptPath, string inputFile, string outputFile, string[] desiredColumns)
        {
            try
            {
                // Combine arguments
                var arguments = $"\"{scriptPath}\" \"{inputFile}\" \"{outputFile}\" {string.Join(" ", desiredColumns)}";

                // Start the process
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = pythonPath,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = new Process { StartInfo = startInfo })
                {
                    process.Start();

                    // Read output and error streams
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();

                    process.WaitForExit();

                    if (process.ExitCode == 0)
                    {
                        Console.WriteLine("Python script executed successfully.");
                        Console.WriteLine(output);
                    }
                    else
                    {
                        Console.WriteLine("Python script execution failed.");
                        Console.WriteLine(error);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
