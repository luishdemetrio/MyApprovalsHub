
using System.Collections.Concurrent;

namespace MyApprovalsHub.Common.ExternalSources.ServiceNow;

public class ServiceNowPendingApprovalDetails
{

    public ConcurrentBag<ApprovalDetail> result { get; set; } = new();

   
    
}

public record ApprovalDetail(string sys_id,
                                string number,
                                string short_description,
                                string assigned_to,
                                int impact,
                                int priority,
                                DateTime opened_at,
                                string description);