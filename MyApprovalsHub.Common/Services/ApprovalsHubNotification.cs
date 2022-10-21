using MyApprovalsHub.Common.Models;
using RestSharp;

namespace MyApprovalsHub.Common.Models;

public class ApprovalsHubNotification
{

    public static bool NotifyUser(string botURL, PendingApproval pendingApproval)
    {
        var client = new RestClient(botURL);

        var request = new RestRequest();

        request.Method = Method.Post;

        request.AddHeader("Accept", "application/json");
        request.AddHeader("Content-Type", "application/json");

        var body = System.Text.Json.JsonSerializer.Serialize(pendingApproval);

        request.AddParameter("application/json", body, ParameterType.RequestBody);

        RestResponse response = client.Execute(request);

        return response.StatusCode == System.Net.HttpStatusCode.OK;
    }

}
