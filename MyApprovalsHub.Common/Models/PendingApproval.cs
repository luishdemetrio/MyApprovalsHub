namespace MyApprovalsHub.Common.Models;

public class PendingApproval
{
    public string? Number { get; set; }
    public string? Description { get; set; }
    public string? Source { get; set; }
    public string? RequestorName { get; set; }
    public DateTime ApprovedOnDate { get; set; }
    public DateTime Date { get; set; }
    public DateTime OpenedAt { get; set; }
    public string? RequestorEmail { get; set; }
    public string? SourcePhoto { get; set; }
    public string? State { get; set; }
    public string? SysId { get; set; }
    public string? Comments { get; set; }
    public string? Impact { get; set; }
    public string? SysApproval { get; set; }
    public string? DetailsUrl { get; set; }
    public string? ApproverName { get; set; }
    public string? Priority { get; set; }
    public string? ShortDescription { get; set; }
    public string? ApproverEmail { get; set; }
}