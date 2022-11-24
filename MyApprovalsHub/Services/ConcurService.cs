using MyApprovalsHub.Common.Interfaces;
using MyApprovalsHub.Common.Models;

namespace MyApprovalsHub.Services;

public class ConcurService : IExternalService
{

    public bool Approve(PendingApproval pendingApproval)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<PendingApproval> GetPendingApprovals(string approverName, string approverEmail)
    {

        return new[]
        {
            new PendingApproval
            {
                Number ="1", 
                Description = "London Sales Conference",
                Source =nameof(PendingApprovalSource.Concur),
                RequestorName = "Hermione Granger",
                Date = new DateTime(2022, 09, 02),
                RequestorEmail = "granger@luisdemetrio.com",
                SourcePhoto = "concur.png",
                State = "Requested"
            },
            new PendingApproval
            {
                Number ="2",
                Description = "Internet May 2022",
                Source =nameof(PendingApprovalSource.Concur),
                RequestorName = "Draco Malfoy",
                Date = new DateTime(2022, 09, 02),
                RequestorEmail = "malfoy@luisdemetrio.com",
                SourcePhoto = "concur.png",
                State = "Requested"
            }
        };
    }

    public bool Reject( PendingApproval pendingApproval)
    {
        throw new NotImplementedException();
    }

    
}
