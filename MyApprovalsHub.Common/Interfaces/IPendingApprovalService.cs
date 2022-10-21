
using MyApprovalsHub.Common.Models;

namespace MyApprovalsHub.Common.Interfaces;

public interface IExternalService
{
    IEnumerable<PendingApproval> GetPendingApprovals(string approverName, string approverEmail);

    bool Approve(PendingApproval pendingApproval);

    bool Reject(PendingApproval pendingApproval);
}
