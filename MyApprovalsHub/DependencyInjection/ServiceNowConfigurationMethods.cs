using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Microsoft.TeamsFx.Helper;
using Microsoft.TeamsFx;
using MyApprovalsHub.Common;

namespace MyApprovalsHub.DependencyInjection;

public static class ServiceNowConfigurationMethods
{
    public static IServiceCollection AddServiceNow(this IServiceCollection services)
    {
        services.AddOptions<ApprovalsHubOptions>()
                              .Configure<IConfiguration>((approvalsHubOptions, configuration) =>
                                  {
                                      approvalsHubOptions.ServiceNowBaseUrl = configuration.GetValue<string>("ServiceNowBaseURL");
                                      approvalsHubOptions.ServiceNowUsername = configuration.GetValue<string>("ServiceNowUsername");
                                      approvalsHubOptions.ServiceNowPassword = configuration.GetValue<string>("ServiceNowPassword");
                                      approvalsHubOptions.ServiceNowClientId = configuration.GetValue<string>("ServiceNowClientId");
                                      approvalsHubOptions.ServiceNowClientSecret = configuration.GetValue<string>("ServiceNowClientSecret");
                                  });

        
        return services;
    }
}
