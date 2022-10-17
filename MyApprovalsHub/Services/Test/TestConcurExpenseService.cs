using MyApprovalsHub.Interfaces;
using MyApprovalsHub.Models;

namespace MyApprovalsHub.Services
{
	public class TestConcurExpenseService : IPendingApprovalService
    {
        public int PendingApprovals { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IEnumerable<PendingApproval> GetPendingApprovals(string approver)
        {

            return new[]
            {
               new PendingApproval
                {
                    Number ="1",
                    Description = "London Sales Conference",
                    Source =nameof(PendingApprovalSource.Concur),
                    Requestor = "Hermione Granger",
                    Date = new DateTime(2022, 09, 02),
                    Email = "granger@luisdemetrio.com",
                    SourcePhoto = "concur.png",
                    State = "Requested"
                },
                new PendingApproval
                {
                    Number ="2",
                    Description = "Internet May 2022",
                    Source =nameof(PendingApprovalSource.Concur),
                    Requestor = "Draco Malfoy",
                    Date = new DateTime(2022, 09, 02),
                    Email = "malfoy@luisdemetrio.com",
                    SourcePhoto = "concur.png",
                    State = "Requested"
                }
            };
        }
    }
}
