using Microsoft.Extensions.Configuration;
using MyApprovalsHub.Common.ExternalSources.ServiceNow;
using MyApprovalsHub.Common.Interfaces;
using MyApprovalsHub.Common.Models;
using Newtonsoft.Json;
using RestSharp;

namespace MyApprovalsHub.Mock.Services.ServiceNow;

public class ServiceNowMock : IServiceNowService
{

    private string _serviceNowInstanceUrl = string.Empty;
    
    private string _ApprovalsHubBotNotificationUrl = string.Empty;

    private string? _currentDirectory = string.Empty ;

    private ServiceNowMock()
    {
            
    }
    public ServiceNowMock(IConfiguration configuration)
    {
        _serviceNowInstanceUrl = configuration.GetSection("ServiceNowBaseURL").Value;
        _ApprovalsHubBotNotificationUrl = configuration.GetSection("ApprovalsHubBotNotificationUrl").Value;

        _currentDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        
    }

    private string GetServiceNowUser(string email)
    {
        return "46d96f57a9fe198101947a9620895886";
    }

    private ServiceNowPendingApprovals GetPendingApprovalsRestAPI(string sysId)
    {

        using StreamReader streamReader = new StreamReader($"{_currentDirectory}\\Services\\ServiceNow\\json\\pendingapprovals.json");

        var r = JsonConvert.DeserializeObject<ServiceNowPendingApprovals>(streamReader.ReadToEnd())!;

        return r;
    }

    private ServiceNowPendingApprovalDetails GetPendingApprovalDetailsRestAPI()
    {
        ServiceNowPendingApprovalDetails details = new();

        using StreamReader streamReader = new StreamReader($"{_currentDirectory}\\Services\\ServiceNow\\json\\servicenowpendingapprovalsdetails.json");

        var r = JsonConvert.DeserializeObject<ServiceNowPendingApprovalDetails>(streamReader.ReadToEnd())!;

        if (r != null && r.result?.Count() > 0)
        {
            details.result = r.result;

        }

        return details;
    }

    private ServiceNowUser GetUserDetail(string userSysIds)
    {

        using StreamReader streamReader = new StreamReader($"{_currentDirectory}\\Services\\ServiceNow\\json\\servicenowusersdetails.json");

        ServiceNowUser user = System.Text.Json.JsonSerializer.Deserialize<ServiceNowUser>(streamReader.ReadToEnd())!;

        return user;
    }


    public IEnumerable<PendingApproval> GetPendingApprovals(string approverName, string approverEmail)
    {


        var sys_id = GetServiceNowUser(approverEmail);

        var approvals = GetPendingApprovalsRestAPI(sys_id);

        ServiceNowPendingApprovalDetails approvalDetails = new();


        foreach (var item in GetPendingApprovalDetailsRestAPI().result)
        {
            approvalDetails.result.Add(item);

        }

        SNowCodeValueHelper snowList = new();

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
                         Impact = snowList.Impact.ContainsKey(approvalDetail.impact.ToString()) ? snowList.Impact[approvalDetail.impact.ToString()] : string.Empty,
                         Priority = snowList.Priority.ContainsKey(approvalDetail.priority.ToString()) ? snowList.Priority[approvalDetail.priority.ToString()] : string.Empty,
                        SysApproval = approval.sys_id,
                        DetailsUrl = $"{_serviceNowInstanceUrl}/sp?id=form&sys_id={approvalDetail.sys_id}&table=change_request"
                    };

        return result;


    }

    public bool Approve(PendingApproval pendingApproval)
    {
        pendingApproval.State = "approved";

        return ChangeApprovalStatus(pendingApproval);
    }



    public bool Reject(PendingApproval pendingApproval)
    {
        pendingApproval.State = "rejected";
        return ChangeApprovalStatus(pendingApproval);
    }

    private bool ChangeApprovalStatus(PendingApproval pendingApproval)
    {

        ApprovalsHubNotification.NotifyUser(_ApprovalsHubBotNotificationUrl, pendingApproval);

        return true;
    }

    public class PendingApprovalCollection
    {

        public List<PendingApproval>? result { get; set; } 



    }

}
