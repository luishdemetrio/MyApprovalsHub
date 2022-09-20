using MyApprovalsHub.Models;

namespace MyApprovalsHub.Services
{
    public class ExpenseList
    {

        public static List<PendingApproval> PendingApprovals { get; private set; } = new();

        public static void AddRange(IEnumerable<PendingApproval> approvals)
        {
            PendingApprovals.AddRange(approvals);
        }

        public static void ClearExpenseList()
        {
            PendingApprovals.Clear();
        }

    }
}
