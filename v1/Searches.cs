using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Create_users_from_CSV_file.v1
{
    public static class Searches
    {
        #region Methods      

        /// <summary>
        /// Execute a silmple search using the query string parameters Field (case sensitive), 
        /// Value and Comparator
        /// </summary>
        /// <returns>The list of the retrieved BOs' ids</returns>
        public static bool BasicSearchCompanies(string UserId)
        {
            HttpWebRequest httpWebRequest = Utils.GetHttpWebRequest(20000,
                Utils.TustenaWebAPiBaseUrl,
                $"Company/Search?Select=FF_VIC_ID_UTENTE&Value={UserId}");
            httpWebRequest.Method = "GET";

            // Company/Search?Field=CompanyName&Value=EURO&Comparator=StartsWit 
            // route + the GET verb find the CompanyBOs satisfying the search criteria.
            // ATTENTION: the Field param is case sensitive!

            httpWebRequest.Accept = "application/json";
            string authToken = Utils.Login(Utils.Username, Utils.Password);
            httpWebRequest.Headers.Add(Utils.TustenaAuthToken, authToken);

            HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();

            string serializedJsonResponse = new StreamReader(response.GetResponseStream()).ReadToEnd();
            // List<long> ids = JsonConvert.DeserializeObject<List<long>>(serializedJsonResponse);

            var companies = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(serializedJsonResponse);

            // Check for a match
            bool isMatchFound = companies.Any(company =>
                company.ContainsKey("FF_VIC_ID_UTENTE") && company["FF_VIC_ID_UTENTE"] == UserId);

            if (isMatchFound)
            {
                Console.WriteLine($"CompanyBO found with FF_VIC_ID_UTENTE: {UserId}");
                return true;
            }

            Console.WriteLine("No matching CompanyBO found, proceeding with creation");
            return false;
        }

        /// <summary>
        /// Search by id a set of CompanyBOs by using the OData standard. 
        /// ATTENTION: the params are case sensitive! For v1 camelCase 
        /// must be used (or use the specific header to override the standard
        /// casing).
        /// </summary>
        /// <returns>The list of the retrieved BOs' ids</returns>
        public static string ODataSearchIdsCompanies()
        {
            string filter = "companyName eq 'ACADEMY'";       // filer
            string orderBy = "id asc";                        // ATTETNTION order criteria must be empty or applied to Id field only!
            string skip = "";                                 // items to skip
            string top = "";                                  // max items to return

            HttpWebRequest httpWebRequest = Utils.GetHttpWebRequest(20000,
              Utils.TustenaWebAPiBaseUrl,
              "v1",
              $"Company/SearchIdsByODataCriteria?$filter={filter}&$orderBy={orderBy}&$skip={skip}&$top={top}");
            httpWebRequest.Method = "GET";

            httpWebRequest.Accept = "application/json";
            string authToken = Utils.Login(Utils.Username, Utils.Password);
            httpWebRequest.Headers.Add(Utils.TustenaAuthToken, authToken);

            HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();

            string serializedJsonResponse = new StreamReader(response.GetResponseStream()).ReadToEnd();
            List<long> ids = JsonConvert.DeserializeObject<List<long>>(serializedJsonResponse);

            return $"CompanyBO found ids: [{string.Join(", ", ids)}]";
        }
        /// <summary>
        /// Search a CompanyBO by using the OData standard. ATTENTION: the params 
        /// are case sensitive! For v1 camelCase must be used (or use the specific 
        /// header to override the standard casing).
        /// </summary>
        /// <returns>A list of companyDTOs</returns>
        public static string ODataSearchCompanies()
        {
            string filter = $"companyName eq 'Ferrari'";       // filer
            string orderBy = "companyName asc";               // order criteria
            string skip = "";                                 // items to skip
            string top = "";                                  // max items to return

            HttpWebRequest httpWebRequest = Utils.GetHttpWebRequest(20000,
              Utils.TustenaWebAPiBaseUrl,
              "v1",
              $"Company/SearchByODataCriteria?$filter={filter}&$orderBy={orderBy}&$skip={skip}&$top={top}");
            httpWebRequest.Method = "GET";

            httpWebRequest.Accept = "application/json";
            string authToken = Utils.Login(Utils.Username, Utils.Password);
            httpWebRequest.Headers.Add(Utils.TustenaAuthToken, authToken);

            HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();

            string serializedJsonResponse = new StreamReader(response.GetResponseStream()).ReadToEnd();
            List<dynamic> companyDTOs = JsonConvert.DeserializeObject<List<dynamic>>(serializedJsonResponse);

            return $"CompanyBOs: [{string.Join(", ", companyDTOs.Select(comp => comp.companyName))}]";
        }

        #endregion
    }
}
