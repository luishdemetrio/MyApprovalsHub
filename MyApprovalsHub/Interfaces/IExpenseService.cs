using Microsoft.Graph;
using MyApprovalsHub.Models;

namespace MyApprovalsHub.Interfaces
{
    public interface IApprovalRequestService
    {

        IEnumerable<PendingApproval> GetPendingApprovals(string approverEmail);
    }
}
