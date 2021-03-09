using Microsoft.Xrm.Sdk;
using System;
using System.ServiceModel;


namespace BasicPlugin
{
    public class MyPlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            // Obtain the execution context from the service provider.  
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            // The InputParameters collection contains all the data passed in the message request.  
            if (context.PrimaryEntityName == Configuration.PrimaryEntityName
                && context.InputParameters.TryGetValue("Target", out var mayBeEntity)
                && mayBeEntity is Entity entity)
            {
                // Obtain the tracing service
                var tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
                try
                {
                    // Obtain the organization service reference which you will need for  
                    // web service calls.  

                    var serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                    var service = serviceFactory.CreateOrganizationService(context.UserId);
                    var timeEntryHandler = new TimeEntryHandler(service);
                    timeEntryHandler.PopulateFields(entity);
                }
                catch (FaultException<OrganizationServiceFault> ex)
                {
                    tracingService.Trace("MyPlugin: {0}", ex.ToString());
                    throw new InvalidPluginExecutionException("An error occurred in MyPlugin.", ex);
                }
                catch (Exception ex)
                {
                    tracingService.Trace("MyPlugin: {0}", ex.ToString());
                    throw;
                }
            }
        }

        
    }
}
