namespace MyApprovalsHub.Models
{
    public record PendingApproval(
        int Id, 
        string Description, 
        string Source, 
        string Requestor, 
        DateTime Date, 
        string Email,
        string SourcePhoto);
}
