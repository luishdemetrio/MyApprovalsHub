using MyApprovalsHub.Common;
using MyApprovalsHub.Common.Interfaces;
using MyApprovalsHub.Common.Models;
using MyApprovalsHub.Services;


namespace MyApprovalsHub.ViewModel;

public class WelcomeViewModel 
{
    private string _approverName;
    private string _approverEmail;

    private ApprovalsHubOptions _config;

    IExternalService expenseService;

    public WelcomeViewModel(string approverName, string approverEmail, ApprovalsHubOptions config, IServiceNowService serviceNowService)
    {
        _approverName = approverName;
        _approverEmail = approverEmail;
        _config = config;
        expenseService = serviceNowService; // new ServiceNowService(_config);
    }

    public delegate void NotifyPendingApprovalTotal(IEnumerable<PendingApproval> pendingApprovals);

 

    public async void  GetServiceNowPendingApprovals(NotifyPendingApprovalTotal serviceNowPendingApprovalsTotal)
    {
      
        var t = Task.Run(() =>
        {
           
           var expenses = expenseService.GetPendingApprovals(_approverName, _approverEmail);

            serviceNowPendingApprovalsTotal?.Invoke(expenses);
        });

        await t.ConfigureAwait(false);

        
    }


    public async void GetConcurPendingApprovals(NotifyPendingApprovalTotal serviceNowPendingApprovalsTotal)
    {

        var t = Task.Run(() =>
        {
            IExternalService expenseService = new ConcurService();

            var expenses = expenseService.GetPendingApprovals(_approverName, _approverEmail);

            serviceNowPendingApprovalsTotal?.Invoke(expenses);
        });

        await t.ConfigureAwait(false);


    }


}
