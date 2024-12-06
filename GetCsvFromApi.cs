using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Create_users_from_CSV_file
{
    public class GetCsvFromApi
    {
        public static void DownloadCsvFile(string url, string outputPath)
        {
            using (WebClient client = new WebClient())
            {
                client.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes("")));
                client.DownloadFile(url, outputPath);
                Console.WriteLine($"CSV file downloaded to {outputPath}");
            }
        }
    }
}
