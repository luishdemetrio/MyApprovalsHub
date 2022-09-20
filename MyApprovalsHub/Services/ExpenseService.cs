using MyApprovalsHub.Interfaces;
using MyApprovalsHub.Models;
using System.Reflection;

namespace MyApprovalsHub.Services;

public abstract class ApprovalRequestService : IApprovalRequestService
{

    protected IConfigurationRoot _configurationRoot;

    protected string ClientId { get; set; }

    protected string ClientSecret { get; set; }

    protected string BaseUrl { get; set; }

    public ApprovalRequestService()
    {
        // Locate the configuration file path at the root of the test project, relative from where these assemblies were deployed
        var configurationJsonFilePath = Path.Combine(Path.GetDirectoryName(typeof(ApprovalRequestService).GetTypeInfo().Assembly.Location) ?? string.Empty, "../../..");
        _configurationRoot = new ConfigurationBuilder()
            .SetBasePath(configurationJsonFilePath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .Build();
    }

    public abstract IEnumerable<PendingApproval> GetPendingApprovals(string approverEmail);


}
