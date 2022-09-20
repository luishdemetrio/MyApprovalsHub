namespace MyApprovalsHub.Models
{
    public record PendingApproval(
        string Number, 
        string Description, 
        string Source, 
        string Requestor, 
        DateTime Date, 
        string Email,
        string SourcePhoto,
        string State);
}
