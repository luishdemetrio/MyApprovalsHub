﻿using MyApprovalsHub.Common;
using MyApprovalsHub.Interfaces;
using MyApprovalsHub.Models;
using RestSharp;

namespace MyApprovalsHub.Services;

public class ConcurService : IPendingApprovalService
{

    
    private readonly string _accessTokenUrl;

    public bool Approve(PendingApproval pendingApproval)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<PendingApproval> GetPendingApprovals(string approverName, string approverEmail)
    {

        //var client = new RestClient("https://www.concursolutions.com/api/v3.0/expense/reports?approverLoginID=luisdem&approvalStatusCode=A_FILE");

        //var request = new RestRequest();

        //request.Method = Method.Get;

        //request.AddHeader("Authorization", $"Bearer {GetToken()}");


        //RestResponse response = client.Execute(request);

        //if (response.StatusCode == System.Net.HttpStatusCode.OK)
        //{

        //}
        //Console.WriteLine(response.Content);

        return new[]
        {
            new PendingApproval
            {
                Number ="1", 
                Description = "London Sales Conference",
                Source =nameof(PendingApprovalSource.Concur),
                RequestorName = "Hermione Granger",
                Date = new DateTime(2022, 09, 02),
                RequestorEmail = "granger@luisdemetrio.com",
                SourcePhoto = "concur.png",
                State = "Requested"
            },
            new PendingApproval
            {
                Number ="2",
                Description = "Internet May 2022",
                Source =nameof(PendingApprovalSource.Concur),
                RequestorName = "Draco Malfoy",
                Date = new DateTime(2022, 09, 02),
                RequestorEmail = "malfoy@luisdemetrio.com",
                SourcePhoto = "concur.png",
                State = "Requested"
            }
        };
    }

    public bool Reject( PendingApproval pendingApproval)
    {
        throw new NotImplementedException();
    }

    private string GetToken()
    {
        var client = new RestClient("https://us.api.concursolutions.com/oauth2/v0/token");
        //client.Timeout = -1;
        var request = new RestRequest();
        request.Method = Method.Post;
        request.AddHeader("Authorization", "Basic dGVzdDp0ZXN0");
        request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
        request.AddParameter("grant_type", "client_credentials");
        request.AddParameter("client_id", "sap");
        request.AddParameter("client_secret", "test");

        RestResponse response = client.Execute(request);
        Console.WriteLine(response.Content);

        return "TOKEN";
    }


    
   



    //public async Task CallWebApiAndProcessResultASync(string webApiUrl, string accessToken, Action<JsonNode> processResult)
    //{
    //    if (!string.IsNullOrEmpty(accessToken))
    //    {
    //        var defaultRequestHeaders = _httpClient.DefaultRequestHeaders;
    //        if (defaultRequestHeaders.Accept == null || !defaultRequestHeaders.Accept.Any(m => m.MediaType == "application/json"))
    //        {
    //            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    //        }
    //        defaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

    //        HttpResponseMessage response = await _httpClient.GetAsync(webApiUrl);
    //        if (response.IsSuccessStatusCode)
    //        {
    //            string json = await response.Content.ReadAsStringAsync();
    //            JsonNode result = JsonNode.Parse(json);
    //            Console.ForegroundColor = ConsoleColor.Gray;
    //            processResult(result);
    //        }
    //        else
    //        {
    //            Console.ForegroundColor = ConsoleColor.Red;
    //            Console.WriteLine($"Failed to call the web API: {response.StatusCode}");
    //            string content = await response.Content.ReadAsStringAsync();

    //            // Note that if you got reponse.Code == 403 and reponse.content.code == "Authorization_RequestDenied"
    //            // this is because the tenant admin as not granted consent for the application to call the Web API
    //            JsonNode result = JsonNode.Parse(content);
    //        }

    //    }
    //}
}
