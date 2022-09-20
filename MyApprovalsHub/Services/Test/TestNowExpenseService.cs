using MyApprovalsHub.Interfaces;
using MyApprovalsHub.Models;

namespace MyApprovalsHub.Services
{
    public class TestNowExpenseService : IPendingApprovalService
    {
        public int PendingApprovals { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IEnumerable<PendingApproval> GetPendingApprovals(string approver)
        {

            return new[]
            {
                new PendingApproval("3", "Internet May 2022", "Service Now", "Harry Potter" , new DateTime(2022, 09, 14), "harry@luisdemetrio.com", "servicenow.png", "Requested")
            };
        }
    }
}