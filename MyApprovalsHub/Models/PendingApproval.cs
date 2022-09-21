namespace MyApprovalsHub.Models;

public class PendingApproval
{
    public string Number { get; set; }
    public string Description { get; set; }
    public string Source { get; set; }
    public string Requestor { get; set; }
    public DateTime Date { get; set; }
    public string Email { get; set; }
    public string SourcePhoto { get; set; }
    public string State { get; set; }

    
}

