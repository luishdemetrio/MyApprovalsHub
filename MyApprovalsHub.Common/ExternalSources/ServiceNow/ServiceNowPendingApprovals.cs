
namespace MyApprovalsHub.Common.ExternalSources.ServiceNow;

public  class ServiceNowPendingApprovals
{
    public Pending[]? result { get; set; } 

}

public record Pending(string sys_id, string sysapproval, DateTime due_date, string state);

//public class Pending
//{
//    public string sys_id { get; set; }
//    public string sysapproval { get; set; }
//    public DateTime due_date { get; set; }
//    public string state { get; set; }
//}