using MyApprovalsHub.Interfaces;
using MyApprovalsHub.Models;

namespace MyApprovalsHub.Services
{
	public class TestConcurExpenseService : IApprovalRequestService
    {
        public IEnumerable<PendingApproval> GetPendingApprovals(string approver)
        {

            return new[]
            {
                new PendingApproval(1, "London Sales Conference", nameof(PendingApprovalSource.Concur), "Hermione Granger",  new DateTime(2022, 09, 02) , "granger@luisdemetrio.com","concur.png" ),
                new PendingApproval(2, "Internet May 2022", nameof(PendingApprovalSource.Concur), "Draco Malfoy" ,  new DateTime(2022, 09, 10), "malfoy@luisdemetrio.com","concur.png")
            };
        }
    }
}
