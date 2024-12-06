using Digita.Tustena.Interfaces;
using System;
using System.Collections.Generic;
using Create_users_from_CSV_file.v1;

namespace Create_users_from_CSV_file
{
    public class InsertIntoCRM 
    {

        public void InsertUsersIntoCRM(List<UserColumns> users)
        {
            foreach (var user in users)
            {
                try
                {
                    Console.WriteLine($"Inserting user {user.UserId}: {user.FirstName} {user.LastName} into the CRM...");
                    //v1.Searches.ODataSearchCompanies();
                    if (!v1.Searches.BasicSearchCompanies(user.UserId))
                    {
                        v1.CreateCompany.CreateCompanyCRM(user);
                        Console.WriteLine($"Successfully inserted user {user.UserId}.");
                    }
                    else
                    {
                        long companyId = Utils.RetrieveCompanyID(user.UserId);
                        v1.UpdateCompany.ModifyCompany(companyId, user);
                        Console.WriteLine($"Successfully updated user {user.UserId}.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error inserting user {user.UserId}: {ex.Message}");
                }
            }
        }
    }
}
