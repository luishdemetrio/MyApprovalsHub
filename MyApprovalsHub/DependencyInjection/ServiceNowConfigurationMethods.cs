﻿using Microsoft.Extensions.Options;
using MyApprovalsHub.Common;
using MyApprovalsHub.Common.Interfaces;
using MyApprovalsHub.Mock.Services.ServiceNow;
using MyApprovalsHub.Services;
using ServiceNow.Api.Tables;

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
                                      approvalsHubOptions.ApprovalsHubBotNotificationUrl = configuration.GetValue<string>("ApprovalsHubBotNotificationUrl");
                                      approvalsHubOptions.ServiceNowUseMockService = configuration.GetValue<bool>("ServiceNowUseMockService");


                                  });

        

        
            
        
        return services;

    }

    public static IServiceCollection SetServiceNowEnvironment(this IServiceCollection services, bool useMock)
    {
        
        if (useMock)
        {
            services.AddScoped<IServiceNowService, ServiceNowMock>();
        }
        else
        {
            services.AddScoped<IServiceNowService, ServiceNowService>();
        }


        return services;

    }
}
