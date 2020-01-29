
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Issuetrak.DataModels.DTOs;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema; 
using Issuetrak.API.Data.Models.Models.OrganizationModels;
using Issuetrak.API.Client;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;

namespace Issuetrak.API.Client.Example
{
    class UFII : Program
    {
        private static string APIKey
        {
            get
            {
                return ConfigurationManager.AppSettings["apiKey"];
            }
        }
        private static int APIVersion
        {
            get
            {
                return Int16.Parse(ConfigurationManager.AppSettings["apiVersion"]);
            }
        }
        private static string BaseApiurl
        {
            get
            {
                return ConfigurationManager.AppSettings["url"];
            }
        }
        private static string Username
        {
            get
            {
                return ConfigurationManager.AppSettings["username"];
            }
        }

        public bool ResponseStatus(HttpStatusCode hsc)
        {
            return ((int)hsc >= 200) && ((int)hsc <= 299); 
        }

        public async override void Execute()
        {
            //upload new issue
            using (IssuetrakAPIClient client = new IssuetrakAPIClient(APIKey))
            {
                CreateIssueDTO createIssueDTO = new CreateIssueDTO()
                {
                    ShouldSuppressEmailForCreateOperation = false,
                    EnteredBy = Username,
                    SubmittedBy = "admin",
                    SubmittedDate = DateTime.Now,
                    Subject = "something",
                    Description = "Testing api ..",
                    IssueTypeID = 1,
                    IssueSubTypeID = 4,
                    PriorityID = 4,
                    OrganizationID = 1
                };

                Console.WriteLine("Creating Issue");

                CreateIssueRequest request = new CreateIssueRequest(BaseApiurl, APIVersion, createIssueDTO);
                dynamic response = client.CreateIssueAsync(request);

                try
                {
                    if (ResponseStatus(response.Result.ResponseStatusCode))
                    {
                        Console.WriteLine("Successful. ID is " + response.Result.ResponseText);
                    }
                    else
                    {
                        Console.WriteLine("Error: " + response.Result.ReasonPhrase);
                        Console.WriteLine(response.Result.ResponseText);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("The following Exception was raised : {0}", e.Message); 
                }
                
                return;
            }
        }
    }
}
