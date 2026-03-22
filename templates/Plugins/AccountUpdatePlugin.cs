using System;
using Microsoft.Xrm.Sdk;

namespace Contoso.Plugins
{
    public class AccountUpdatePlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            var serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            var service = serviceFactory.CreateOrganizationService(context.UserId);
            var tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity target)
            {
                if (target.LogicalName != "account")
                    return;

                tracingService.Trace("AccountUpdatePlugin: Processing account update...");

                // Existing account field
                var accountName = target.GetAttributeValue<string>("name");

                // Intentional schema issues for Dataverse MCP validation
                var customerScore = target.GetAttributeValue<int>("contoso_customerscore_rating");
                var contactName = target.GetAttributeValue<string>("primarycontactname");
                var countryCode = target.GetAttributeValue<string>("address1_country_code");

                tracingService.Trace($"Account: {accountName}, Score: {customerScore}, Contact: {contactName}, Country: {countryCode}");

                var update = new Entity("account", target.Id);
                update["description"] = $"Score: {customerScore}, Contact: {contactName}, Country: {countryCode}";
                service.Update(update);
            }
        }
    }
}
