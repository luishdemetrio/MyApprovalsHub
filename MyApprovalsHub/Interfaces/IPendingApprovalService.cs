using Microsoft.Graph;
using MyApprovalsHub.Models;

namespace MyApprovalsHub.Interfaces
{
    public interface IPendingApprovalService
    {
        public int PendingApprovals { get; set; }

        IEnumerable<PendingApproval> GetPendingApprovals(string approverEmail);
    }
}
