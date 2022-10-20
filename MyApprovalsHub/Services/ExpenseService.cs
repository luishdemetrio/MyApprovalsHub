using MyApprovalsHub.Common;
using MyApprovalsHub.Interfaces;

namespace MyApprovalsHub.Services;

public abstract class ApprovalRequestService : IPendingApprovalService
{

    protected string ClientId { get; set; }

    protected string ClientSecret { get; set; }

    protected string BaseUrl { get; set; }
 
    public abstract IEnumerable<PendingApproval> GetPendingApprovals(string approverEmail);

    public abstract bool Approve( PendingApproval pendingApproval);

    public abstract bool Reject( PendingApproval pendingApproval);

}
