using MyApprovalsHub.Common;
using Newtonsoft.Json;
using RestSharp;

namespace MyApprovalsHub.Services
{
    public class ApprovalsHubNotification
    {

        public static bool NotifyUser(PendingApproval pendingApproval)
        {
            var client = new RestClient("https://myapprovalhubbotbot.azurewebsites.net/api/notification");

            var request = new RestRequest();

            request.Method = Method.Post;

            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json");


            //var body = @"{""UserPrincipalName"": ""luffy@luisdemetrio.com""}";

            var body = System.Text.Json.JsonSerializer.Serialize(pendingApproval);

            request.AddParameter("application/json", body, ParameterType.RequestBody);

            RestResponse response = client.Execute(request);

            return response.StatusCode == System.Net.HttpStatusCode.OK;
        }

    }
}
