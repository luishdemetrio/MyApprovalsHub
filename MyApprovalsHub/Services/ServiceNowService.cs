using MyApprovalsHub.Common.ExternalSources.ServiceNow;
using MyApprovalsHub.Common.Interfaces;
using MyApprovalsHub.Common.Models;
using Newtonsoft.Json;
using RestSharp;

namespace MyApprovalsHub.Services;

public class ServiceNowService : IServiceNowService
{
    private string _serviceNowInstanceUrl;
    private string _serviceNowUsername;
    private string _serviceNowPassword;
    private string _serviceNowClientId;
    private string _serviceNowClientSecret;

    private string _ApprovalsHubBotNotificationUrl;

    private static Dictionary<int, string> _impacts ;

    private static Dictionary<int, string> _priorities;

    public ServiceNowService(IConfiguration config)
    {

        _serviceNowInstanceUrl = config.GetValue<string>("ServiceNowBaseUrl");
        _serviceNowUsername = config.GetValue<string>("ServiceNowUsername");
        _serviceNowPassword = config.GetValue<string>("ServiceNowPassword");

        _serviceNowClientId = config.GetValue<string>("ServiceNowClientId");
        _serviceNowClientSecret = config.GetValue<string>("ServiceNowClientSecret");

        _ApprovalsHubBotNotificationUrl = config.GetValue<string>("ApprovalsHubBotNotificationUrl");

        AppSettingsValidation();

        PopulateImpacts();

        PopulatePriorities();
    }

    private void AppSettingsValidation()
    {
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

        if (string.IsNullOrWhiteSpace(_ApprovalsHubBotNotificationUrl))
        {
            throw new Exception($"{nameof(_ApprovalsHubBotNotificationUrl)} must be set.");
        }
    }

    private void PopulateImpacts()
    {
        _impacts = new();

        _impacts.Add(1, "High");
        _impacts.Add(2, "Medium");
        _impacts.Add(3, "Low");

    }

    private void PopulatePriorities()
    {
        _priorities = new();

        _priorities.Add(1, "Critical");
        _priorities.Add(2, "High");
        _priorities.Add(3, "Moderate");
        _priorities.Add(4, "Low");

    }

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

            if (dic != null && dic.ContainsKey("access_token") == true)
            {
                token = dic["access_token"];
            }
        }

        return token;
    }

    private string GetServiceNowUser(string email)
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

            if (dynamicObject.result.Count > 0)
            {
                var item = dynamicObject.result[0].sys_id;
                sys_id = item;
            }
            // else
            // {
            //user not found
            //}
        }

        return sys_id;
    }

    private ServiceNowPendingApprovals GetPendingApprovalsRestAPI(string sysId)
    {

        ServiceNowPendingApprovals approvals = new();

        //var client = new RestClient($"{_serviceNowInstanceUrl}/api/now/table/sysapproval_approver?approver={sysId}&state=requested&sysparm_exclude_reference_link=true&sysparm_fields=state%2Csys_created_by%2Csysapproval");
        var client = new RestClient($"{_serviceNowInstanceUrl}/api/now/table/sysapproval_approver?sysparm_query=approver%3D{sysId}%5EstateNOT%20INapproved%2Crejected%2Ccancelled%2Cnot_required&sysparm_exclude_reference_link=true&sysparm_fields=sysapproval,due_date,state,sys_id");

        var request = new RestRequest();

        request.Method = Method.Get;

        request.AddHeader("Authorization", $"Bearer {GetToken()}");

        RestResponse response = client.Execute(request);

        if (response != null && response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            var r = JsonConvert.DeserializeObject<ServiceNowPendingApprovals>(response.Content)!;

            approvals = r;
        }

        return approvals;
    }

    private ServiceNowPendingApprovalDetails GetPendingApprovalDetailsRestAPI(string approvalId)
    {
        ServiceNowPendingApprovalDetails details = new();

        var client = new RestClient($"{_serviceNowInstanceUrl}/api/now/table/task?sysparm_query=sys_idIN{approvalId}&sysparm_exclude_reference_link=true&sysparm_fields=number%2Cshort_description%2Cassigned_to%2Csys_id%2Cimpact%2Copened_at%2Cpriority%2Cdescription");
        var request = new RestRequest();

        request.Method = Method.Get;

        request.AddHeader("Authorization", $"Bearer {GetToken()}");

        RestResponse response = client.Execute(request);

        if (response != null && response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            var r = JsonConvert.DeserializeObject<ServiceNowPendingApprovalDetails>(response.Content)!;

            if (r != null && r.result.Count() > 0)
            {
                details.result = r.result;
                
            }

        }

        return details;
    }

    private ServiceNowUser GetUserDetail(string userSysIds)
    {
        ServiceNowUser user = null;

        var client = new RestClient($"{_serviceNowInstanceUrl}/api/now/table/sys_user?sysparm_query=sys_idIN{userSysIds}&sysparm_fields=sys_id,name,email");

        var request = new RestRequest();

        request.Method = Method.Get;

        request.AddHeader("Authorization", $"Bearer {GetToken()}");

        RestResponse response = client.Execute(request);

        if (response != null && response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            user = JsonConvert.DeserializeObject<ServiceNowUser>(response.Content)!;

        }

        return user;
    }


    private bool ChangeApprovalStatus(  PendingApproval pendingApproval)
    {

        var client = new RestClient($"{_serviceNowInstanceUrl}/api/now/table/sysapproval_approver/{pendingApproval.SysApproval}");
                
        var request = new RestRequest();

        request.Method = Method.Put;

        request.AddHeader("Accept", "application/json");
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Authorization", $"Bearer {GetToken()}");

        
        var body = @"{""state"":""" + pendingApproval.State + @""",""comments"":""" + pendingApproval.Comments + @""",""approval_source"":""MyApprovalsHub""}";

        request.AddParameter("application/json", body, ParameterType.RequestBody);

        RestResponse response = client.Execute(request);

        bool statusCode = response.StatusCode == System.Net.HttpStatusCode.OK;

        ApprovalsHubNotification.NotifyUser(_ApprovalsHubBotNotificationUrl, pendingApproval);

        return statusCode;
    }

    public bool Approve( PendingApproval pendingApproval)
    {
        pendingApproval.State = "approved";
        
        return ChangeApprovalStatus(  pendingApproval);
    }
    public bool Reject(PendingApproval pendingApproval)
    {
        pendingApproval.State = "rejected";
        return ChangeApprovalStatus(  pendingApproval);
    }

    public IEnumerable<PendingApproval> GetPendingApprovals(string approverName, string approverEmail)
    {

        var sys_id = GetServiceNowUser(approverEmail);

        var approvals = GetPendingApprovalsRestAPI(sys_id);

        ServiceNowPendingApprovalDetails approvalDetails = new();

        //we need to chunck the pending approvals to avoid getting an exception due the request is too long
        var chunck = approvals.result?.Select(p => p.sysapproval).ToList().Chunk(10);

        Parallel.ForEach(chunck, p =>
        {
            foreach (var item in GetPendingApprovalDetailsRestAPI(string.Join(",", p)).result)
            {
                approvalDetails.result.Add(item);

            }
        });

        
        //we need to get the name and email of the user
        var users = GetUserDetail(string.Join(",", approvalDetails.result.Select(u => u.assigned_to).Distinct()));


        var result = from approval in approvals.result
                     join approvalDetail in approvalDetails.result
                        on approval.sysapproval equals approvalDetail.sys_id
                     join user in users.result
                        on approvalDetail.assigned_to equals user.sys_id
                     
                     select new PendingApproval
                     {
                         Number = approvalDetail.number,
                         ShortDescription = approvalDetail.short_description,
                         Description = approvalDetail.description,
                         ApproverName = approverName,
                         ApproverEmail = approverEmail,
                         RequestorName = user.name,
                         RequestorEmail = user.email,
                         OpenedAt = approvalDetail.opened_at.Date,
                         Date = approval.due_date.Date,
                         Source = PendingApprovalSource.ServiceNow.ToString(),
                         SourcePhoto = "servicenow.png",
                         State = approval.state,
                         SysId = approvalDetail.sys_id,
                         Impact = _impacts.ContainsKey(approvalDetail.impact) ? _impacts[approvalDetail.impact] : string.Empty,
                         Priority= _priorities.ContainsKey(approvalDetail.priority) ? _priorities[approvalDetail.priority] : string.Empty,
                         SysApproval = approval.sys_id,
                         DetailsUrl = $"{_serviceNowInstanceUrl}/sp?id=form&sys_id={approvalDetail.sys_id}&table=change_request"
                     };


        return result;
             
    }


}



