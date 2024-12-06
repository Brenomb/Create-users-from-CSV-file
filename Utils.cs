using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Create_users_from_CSV_file
{
    public static class Utils
    {
        #region Consts

        public const string TustenaWebAPiBaseUrl = "http://localhost:82/api/";
        public const string Username = "admin@admin.com";
        public const string Password = "admin";
        public const string TustenaAuthToken = "Authorization"; // old token header key name (used in pre-JWT authentication): "TustenaAuthToken"
        public const string TustenaWebApiKey = "";

        #endregion

        #region Properties

        internal static long LastCreatedCompanyBOId { get; set; } = 0;

        #endregion

        #region Methods

        public static HttpClient GetHttpClient(string baseUrl, int timeoutSeconds)
        {
            return GetHttpClient(baseUrl, "v1", timeoutSeconds);
        }

        public static HttpClient GetHttpClient(string baseUrl, string version, int timeoutSeconds)
        {
            // The following SecurityProtocol setting is mandatory to configure .NET environment to enable the use of the 
            // SecurityProtocolType version Tls12 that is required by GS's API (otherwise the default value is Tls 1.0)
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;


            var httpClient = new HttpClient();

            httpClient.Timeout = new TimeSpan(0, 0, 0, timeoutSeconds);
            httpClient.BaseAddress = new Uri($"{baseUrl}{version}/");
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return httpClient;
        }

        /// <summary>
        /// Default method version without the "version" param: the value used is 
        /// "latest" which point to the latest version available
        /// </summary>
        /// <param name="timeoutMilliseconds"></param>
        /// <param name="baseUrl"></param>
        /// <param name="actionPath"></param>
        /// <returns></returns>
        public static HttpWebRequest GetHttpWebRequest(int timeoutMilliseconds, string baseUrl, string actionPath)
        {
            return GetHttpWebRequest(timeoutMilliseconds, baseUrl, "v1", actionPath);
        }

        public static HttpWebRequest GetHttpWebRequest(int timeoutMilliseconds, string baseUrl, string version, string actionPath)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create($"{baseUrl}{version}/{actionPath}");
            httpWebRequest.Timeout = timeoutMilliseconds;
            httpWebRequest.ContentType = "application/json";
            return httpWebRequest;
        }

        public static string Login(string username, string password)
        {
            return Login(username, password, "latest");
        }

        public static string Login(string username, string password, string version)
        {
            string authToken = string.Empty;

            // Build the HttpWebRequest object used as a Web client to execute GET, 
            // POST, PUT, DELETE calls to the WebAPI
            HttpWebRequest httpWebRequest = GetHttpWebRequest(60000, //timeout in milliseconds
                TustenaWebAPiBaseUrl, // base WebAPI URL
                version,
                $"Auth/Login"); // specific WebAPI Controller/Action sub-route

            ASCIIEncoding encoder = new ASCIIEncoding();
            var payload = new
            {
                Grant_Type = "password",
                Username = username,
                Password = password
            };

            byte[] data = encoder.GetBytes(JsonConvert.SerializeObject(payload)); // a json object, or xml, whatever...

            // Set verb for the current request
            httpWebRequest.Method = "POST";
            httpWebRequest.Accept = "application/json";
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.ContentLength = data.Length;
            httpWebRequest.GetRequestStream().Write(data, 0, data.Length);

            // Execute sync request
            HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();

            // Check the response status code: if different from 200 throw an exception.
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"Response status code: {(int)(response.StatusCode)}");
            }

            // Synchronously read response  as JSON formatted string
            string serializedJsonResponse = new StreamReader(response.GetResponseStream()).ReadToEnd();

            // Deserialize serializedJsonResponse in a .NET class
            dynamic jwtDeserializedResponse = JsonConvert.DeserializeObject(serializedJsonResponse);
            authToken = $"Bearer {jwtDeserializedResponse.access_token}";

            return authToken;
        }

        /// <summary>
        /// Gets the authentication token using the HttpClient natively async object. 
        /// To be compatible with Tustena env it must be "wrapped" in a sync execution
        /// context by using the Tustena AsyncHelpers methods.
        /// </summary>
        /// <param name="username">The user account</param>
        /// <param name="password">The user password</param>
        /// <returns>A string representing the authentication token</returns>
        public static async Task<string> LoginAsync(string username, string password)
        {
            string authToken = string.Empty;

            // Build the HttpClient object used as a Web client to execute GET, 
            // POST, PUT, DELETE calls to the WebAPI
            HttpClient httpClient = Utils.GetHttpClient(Utils.TustenaWebAPiBaseUrl, // base WebAPI URL                
                20); // timeout in seconds

            // Specific WebAPI Controller/ Action sub - route, passed later as a param to 
            // the HttpClient.GetAsync() method
            string requestUrl = $"{httpClient.BaseAddress}Auth/Login";

            // ATTENTION: now the HttpClient is brand new and Headers cleaning is redundant,
            // but in some scenarios it is mandatory to clean headers before adding a new one because
            // they are modeled as collections and if a header key is already present the new value 
            // is concatenated to the old one!
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Build request payload
            var payload = new
            {
                Grant_Type = "password",
                Username = username,
                Password = password
            };

            // Request async execution
            //var stringContent = new StringContent(payload.ToString());
            var stringContent = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(requestUrl, stringContent);


            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"Response status code: {(int)(response.StatusCode)}");
            }

            // Response async reading as JSON formatted string
            var responseContent = await response.Content.ReadAsStringAsync();

            dynamic jwtDeserializedResponse = JsonConvert.DeserializeObject(responseContent);
            authToken = $"Bearer {jwtDeserializedResponse.access_token}";

            return authToken;
        }


        #endregion

        public static long RetrieveCompanyID(string userId)
        {
            //http://localhost:82/api/latest/Company/Search?field=CompanyName&value=Ferrari&comparator=Equals
            HttpWebRequest httpWebRequest = Utils.GetHttpWebRequest(20000,
                Utils.TustenaWebAPiBaseUrl,
                "latest",
                $"Company/Search?field=FF_VIC_ID_UTENTE&value={userId}&comparator=Equals");
            httpWebRequest.Method = "GET";
            httpWebRequest.Accept = "application/json";

            string authToken = Utils.Login(Utils.Username, Utils.Password);
            httpWebRequest.Headers.Add(Utils.TustenaAuthToken, authToken);

            var response = httpWebRequest.GetResponse();

            string serializedJsonResponse = new StreamReader(response.GetResponseStream()).ReadToEnd();

            var companies = JsonConvert.DeserializeObject<List<dynamic>>(serializedJsonResponse);
            var matchingCompany = companies.FirstOrDefault(c => c.FF_VIC_ID_UTENTE == userId);

            return matchingCompany.id;
        }
    }
}
