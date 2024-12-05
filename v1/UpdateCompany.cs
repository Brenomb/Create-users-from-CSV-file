using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Create_users_from_CSV_file.v1
{
    public class UpdateCompany
    {

        /// <summary>
        /// Gets the CompanyBO with the given Id and updates the respective BO in the db
        /// </summary>
        /// <param name="id">The BO's Id</param>
        /// <returns>A message indicating if the BO has been successfully modified</returns>
        public static string ModifyCompany(long userId, UserColumns user)
        {
            // *** Get the desired CompanyDTO ***

            HttpWebRequest httpWebRequest = Utils.GetHttpWebRequest(20000,
                Utils.TustenaWebAPiBaseUrl,
                $"Company/{userId}");
            httpWebRequest.Method = "GET";
            httpWebRequest.Accept = "application/json";
            string authToken = Utils.Login(Utils.Username, Utils.Password);
            httpWebRequest.Headers.Add(Utils.TustenaAuthToken, authToken);

            HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();

            string serializedJsonResponse = new StreamReader(response.GetResponseStream()).ReadToEnd();
            dynamic company = JsonConvert.DeserializeObject<dynamic>(serializedJsonResponse);

            // Modify the DTO
            company.FF_VIC_ID_UTENTE = user.UserId;
            string RagioneSociale = $"{user.FirstName} {user.LastName}";
            company.companyName = RagioneSociale;
            company.NaturalPerson = true;
            company.NaturalPersonName = user.FirstName;
            company.NaturalPersonSurname = user.LastName;
            company.emails = user.Email;
            company.TaxIdentificationNumber = user.GovId;
            company.phones = user.CellPhone;
            company.address = user.Address;
            company.city = user.City;
            company.ZipCode = user.PostalCode;
            company.Province = user.Province;
            company.Region = user.Region;

            // *** Update the respective BO in the db ***
            // Serialize the companyDTO first in a JSON string and then in a byte Array
            ASCIIEncoding encoder = new ASCIIEncoding();
            byte[] data = encoder.GetBytes(JsonConvert.SerializeObject(company)); // a json object, or xml, whatever...

            httpWebRequest = Utils.GetHttpWebRequest(20000,
                Utils.TustenaWebAPiBaseUrl,
                $"Company/CreateOrUpdate");
            httpWebRequest.Method = "POST"; // Company/CreateOrUpdate route + the POST verb update the given CompanyDTO in the db
            httpWebRequest.Accept = "application/json";
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.ContentLength = data.Length;
            httpWebRequest.Headers.Add(Utils.TustenaAuthToken, authToken);
            httpWebRequest.GetRequestStream().Write(data, 0, data.Length);

            response = (HttpWebResponse)httpWebRequest.GetResponse();

            serializedJsonResponse = new StreamReader(response.GetResponseStream()).ReadToEnd();
            long modifiedCompanyBOId = JsonConvert.DeserializeObject<long>(serializedJsonResponse);

            return $"Modified CompanyBO {modifiedCompanyBOId}";
        }
    }
}
