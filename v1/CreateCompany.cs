using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Create_users_from_CSV_file.v1
{
    public class CreateCompany
    {
        public static string CreateCompanyCRM(UserColumns user)
        {
            // *** Get an empty CompanyDTO instance ***
            HttpWebRequest httpWebRequest = Utils.GetHttpWebRequest(20000,
                 Utils.TustenaWebAPiBaseUrl,
                 $"Company/GetNewInstance");
            httpWebRequest.Method = "GET"; // Company/GetNewInstance route + GET verb get a new empty CompanyDTO which is not persisted in the db yet
            httpWebRequest.Accept = "application/json";
            string authToken = Utils.Login(Utils.Username, Utils.Password);
            httpWebRequest.Headers.Add(Utils.TustenaAuthToken, authToken);

            HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();

            string serializedJsonResponse = new StreamReader(response.GetResponseStream()).ReadToEnd();
            dynamic company = JsonConvert.DeserializeObject<dynamic>(serializedJsonResponse);

            // Fill the companyDTO properties and save into the db
            //string RagioneSociale = $"{user.FirstName} {user.LastName}";
            //company.companyName = RagioneSociale;
            company.FF_VIC_ID_UTENTE = user.UserId;
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

            if (user.Country == "Italia")
                company.State = "IT";
            else
                company.State = user.Country;

            // Serialize the companyDTO first in a JSON string and then in a byte Array
            ASCIIEncoding encoder = new ASCIIEncoding();
            byte[] data = encoder.GetBytes(JsonConvert.SerializeObject(company)); // a json object, or xml, whatever...

            httpWebRequest = Utils.GetHttpWebRequest(20000,
                Utils.TustenaWebAPiBaseUrl,
                $"Company");
            httpWebRequest.Method = "PUT"; // Company route + the PUT verb insert the given CompanyDTO in the db
            httpWebRequest.Accept = "application/json";
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.ContentLength = data.Length;
            httpWebRequest.Headers.Add(Utils.TustenaAuthToken, authToken);
            httpWebRequest.GetRequestStream().Write(data, 0, data.Length);

            response = (HttpWebResponse)httpWebRequest.GetResponse();

            serializedJsonResponse = new StreamReader(response.GetResponseStream()).ReadToEnd();
            long newCompanyBOId = JsonConvert.DeserializeObject<long>(serializedJsonResponse);

            Utils.LastCreatedCompanyBOId = newCompanyBOId;

            return newCompanyBOId.ToString();
        }
    }
}
