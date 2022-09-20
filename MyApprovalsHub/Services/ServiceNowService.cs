using MyApprovalsHub.Models;
using Newtonsoft.Json;
using RestSharp;

namespace MyApprovalsHub.Services;

public class ServiceNowService : ApprovalRequestService
{

    private string _serviceNowInstanceUrl;
    private string _serviceNowUsername;
    private string _serviceNowPassword;
    private string _serviceNowClientId;
    private string _serviceNowClientSecret;
    
    public ServiceNowService()
    {

        _serviceNowInstanceUrl = base._configurationRoot["ServiceNowInstanceURL"];
        _serviceNowUsername = base._configurationRoot["ServiceNowUsername"];
        _serviceNowPassword = base._configurationRoot["ServiceNowPassword"];

        _serviceNowClientId = base._configurationRoot["ServiceNowClientId"];
        _serviceNowClientSecret = base._configurationRoot["ServiceNowClientSecret"];

        if (string.IsNullOrWhiteSpace(_serviceNowInstanceUrl))
        {
            throw new Exception($"{nameof(_serviceNowInstanceUrl)} must be set.");
        }

        if (string.IsNullOrWhiteSpace(_serviceNowUsername))
        {
            throw new Exception($"{nameof(_serviceNowUsername)} must be set.");
        }

        if (string.IsNullOrWhiteSpace(_serviceNowPassword))
        {
            throw new Exception($"{nameof(_serviceNowPassword)} must be set.");
        }

        if (string.IsNullOrWhiteSpace(_serviceNowClientId))
        {
            throw new Exception($"{nameof(_serviceNowClientId)} must be set.");
        }

        if (string.IsNullOrWhiteSpace(_serviceNowClientSecret))
        {
            throw new Exception($"{nameof(_serviceNowClientSecret)} must be set.");
        }

        
    }        
    
    /*
    {
        "access_token": "PNr9B-GrTQw16p_Gj-05MFd8HHWrtZCarlP18NaY09kVH7_IijCWXgv6ZyrgSiOWXKedHf-VBVm1JpA6ezmdkw",
        "refresh_token": "Rcqu4ebNwDonjuYN9bjIY129mQijO1YOkkA3ccCrs0iu4io-tamAvfmHJJrVJ9j2ckig-M1eHl5HH3S5Qcm6aQ",
        "scope": "useraccount",
        "token_type": "Bearer",
        "expires_in": 1799
    }*/
    private string GetToken()
    {
        string token = string.Empty;

        var client = new RestClient($"{_serviceNowInstanceUrl}/oauth_token.do");
        var request = new RestRequest();
        request.Method = Method.Post;

        request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
        request.AddParameter("client_id", _serviceNowClientId);
        request.AddParameter("client_secret", _serviceNowClientSecret);
        request.AddParameter("grant_type", "password");
        request.AddParameter("username", _serviceNowUsername);
        request.AddParameter("password", _serviceNowPassword);
        
        RestResponse response = client.Execute(request);

        if (response != null && response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            var dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content);

            if ( dic != null && dic.ContainsKey("access_token") == true)
            {
                token = dic["access_token"];
            }
        }
        
        return token;
    }

    //API
    //https://dev52648.service-now.com/api/now/table/sys_user?sysparm_query=email%3Dluke.wilson%40example.com&sysparm_fields=sys_id&sysparm_limit=1
    //RETURN
    //"{\"result\":[{\"sys_id\":\"46d96f57a9fe198101947a9620895886\"}]}"
    public string GetServiceNowUser(string email)
    {
        string sys_id = string.Empty;

        var client = new RestClient($"{_serviceNowInstanceUrl}/api/now/table/sys_user?sysparm_query=email={email}&sysparm_fields=sys_id&sysparm_limit=1");

        var request = new RestRequest();

        request.Method = Method.Get;

        request.AddHeader("Authorization", $"Bearer {GetToken()}");


        RestResponse response = client.Execute(request);

        if (response != null && response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            var dynamicObject = JsonConvert.DeserializeObject<dynamic>(response.Content)!;

            var item = dynamicObject.result[0].sys_id;
            sys_id = item;
        }

        return sys_id;
    }

    //API
    //https://dev52648.service-now.com/api/now/table/sys_user?sysparm_query=email%3Dluke.wilson%40example.com&sysparm_fields=sys_id&sysparm_limit=1
    //RETURN
    //"{\"result\":[{\"sys_id\":\"46d96f57a9fe198101947a9620895886\"}]}"
    public string GetServiceNowPendingApprovals(string sysId)
    {
        string sys_id = string.Empty;

        var client = new RestClient($"{_serviceNowInstanceUrl}/api/now/table/sysapproval_approver?approver={sysId}&state=requested&sysparm_exclude_reference_link=true&sysparm_fields=state%2Csys_created_by%2Csysapproval%2Csys_updated_by");

        var request = new RestRequest();

        request.Method = Method.Get;

        request.AddHeader("Authorization", $"Bearer {GetToken()}");


        RestResponse response = client.Execute(request);

        if (response != null && response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            var dynamicObject = JsonConvert.DeserializeObject<dynamic>(response.Content)!;

            var item = dynamicObject.result[0].sys_id;
            sys_id = item;
        }

        return sys_id;
    }



    public override IEnumerable<PendingApproval> GetPendingApprovals(string approverEmail)
    {
        //"luke.wilson@example.com"
        var sys_id = GetServiceNowUser(approverEmail);




        return new[]
        {
            new PendingApproval(3, "Internet May 2022", "Service Now", "Harry Potter" ,  new DateTime(2022, 09, 14), "harry@luisdemetrio.com", "servicenow.png")
        };
    }
}