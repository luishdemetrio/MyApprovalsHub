namespace MyApprovalsHub.Agent.Models
{
    public class NotificationDefaultModel
    {
        public string Number { get; set; }

        public string ShortDescription { get; set; }

        public string Description { get; set; }

        public string RequestorName { get; set; }

        public string RequestorEmail { get; set; }

        public DateTime OpenedAt { get; set; }

        public DateTime ApprovedOnDate { get; set; }

        public string Status { get; set; }

        public string ApproverName { get; set; }

        public string ApproverEmail { get; set; }

        public string NotificationUrl { get; set; }
    }
}
