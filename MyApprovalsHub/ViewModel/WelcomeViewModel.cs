using Microsoft.AspNetCore.Components;
using MyApprovalsHub.Common;
using MyApprovalsHub.Interfaces;
using MyApprovalsHub.Models;
using MyApprovalsHub.Services;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using static MyApprovalsHub.Services.ServiceNowService.PendingApprovalDetails;

namespace MyApprovalsHub.ViewModel;

public class WelcomeViewModel 
{

    private string _approverEmail;
    private ApprovalsHubOptions _config;
    IPendingApprovalService expenseService;

    public WelcomeViewModel(string approverEmail, ApprovalsHubOptions config)
    {
        _approverEmail = approverEmail;
        _config = config;
        expenseService = new ServiceNowService(_config);
    }

    public delegate void NotifyPendingApprovalTotal(IEnumerable<PendingApproval> pendingApprovals);

 

    public async void  GetServiceNowPendingApprovals(NotifyPendingApprovalTotal serviceNowPendingApprovalsTotal)
    {
      
        var t = Task.Run(() =>
        {
           
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
