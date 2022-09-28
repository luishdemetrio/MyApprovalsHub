using Microsoft.AspNetCore.Components;
using MyApprovalsHub.Interfaces;
using MyApprovalsHub.Models;
using MyApprovalsHub.Services;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using static MyApprovalsHub.Services.ServiceNowService.PendingApprovalDetails;

namespace MyApprovalsHub.ViewModel
{
    public class WelcomeViewModel 
    {

      //  public ConcurrentBag<PendingApproval> PendingApprovals { get; set; } = new();


        private string _approverEmail;

        public delegate void NotifyPendingApprovalTotal(IEnumerable<PendingApproval> pendingApprovals);

        public event NotifyPendingApprovalTotal ServiceNowPendingApprovalsTotal;


       

        public void GetPendingApprovals(string approverEmail)
        {
            _approverEmail = approverEmail;

            GetServiceNow();

        }

        private async void  GetServiceNow()
        {
          
            var t = Task.Run(() =>
            {
                IPendingApprovalService expenseService = ServiceNowService.GetInstance();

               var expenses = expenseService.GetPendingApprovals(_approverEmail);

                //foreach (var item in expenses)
                //{
                //    PendingApprovals.Add(item); 
                //}

                ServiceNowPendingApprovalsTotal?.Invoke(expenses);
            });

            await t.ConfigureAwait(false);

            
        }

        

    }
}
