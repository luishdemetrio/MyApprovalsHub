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
                 new PendingApproval
                {
                    Number ="1",
                    Description = "London Sales Conference",
                    Source =nameof(PendingApprovalSource.Concur),
                    Requestor = "Harry Potter",
                    Date = new DateTime(2022, 09, 02),
                    Email = "harry@luisdemetrio.com",
                    SourcePhoto = "concur.png",
                    State = "Requested"
                }
             
            };
        }
    }
}