using MyApprovalsHub.Common;

namespace MyApprovalsHub.Interfaces;

public interface IPendingApprovalService
{

    IEnumerable<PendingApproval> GetPendingApprovals(string approverEmail);

    bool Approve( PendingApproval pendingApproval);

    bool Reject( PendingApproval pendingApproval);
}
