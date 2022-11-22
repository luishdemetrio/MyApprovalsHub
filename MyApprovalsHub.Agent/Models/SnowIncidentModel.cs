namespace MyApprovalsHub.Agent.Models;

public class SnowIncidentModel {
    public string CaseID { get; set; }
    public string Title { get; set; }
    public DateTime OpenedAt { get; set; }
    public string ShortDescription { get; set; }
    public string LongDescription { get; set; }
    public string Description { get; set; }
    public string Requestor { get; set; }
    public string ViewDetailsUrl { get; set; }
    public string Impact { get; set; }
    public string Urgency { get; set; }
    public string Category { get;}
    public string Priority { get; set; }
    public string Subcategory { get; set; }
    public string State { get; set; }
}