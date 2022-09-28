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

        private string _approverEmail;

        public WelcomeViewModel(string approverEmail)
        {
            _approverEmail = approverEmail;
        }
    
        public delegate void NotifyPendingApprovalTotal(IEnumerable<PendingApproval> pendingApprovals);

     
        public async void  GetServiceNowPendingApprovals(NotifyPendingApprovalTotal serviceNowPendingApprovalsTotal)
        {
          
            var t = Task.Run(() =>
            {
                IPendingApprovalService expenseService = ServiceNowService.GetInstance();

               var expenses = expenseService.GetPendingApprovals(_approverEmail);

                serviceNowPendingApprovalsTotal?.Invoke(expenses);
            });

            await t.ConfigureAwait(false);

            
        }


        public async void GetConcurPendingApprovals(NotifyPendingApprovalTotal serviceNowPendingApprovalsTotal)
        {

            var t = Task.Run(() =>
            {
                IPendingApprovalService expenseService = new ConcurService();

                var expenses = expenseService.GetPendingApprovals(_approverEmail);

                serviceNowPendingApprovalsTotal?.Invoke(expenses);
            });

            await t.ConfigureAwait(false);


        }


    }
}
