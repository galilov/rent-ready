using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.ServiceModel;
using System.Linq;

namespace BasicPlugin
{
    public class MyPlugin : IPlugin
    {
        private const string PrimaryEntityName = "msdyn_timeentry";
        private static readonly AttrWrapper<DateTime> Start = new AttrWrapper<DateTime>("msdyn_start");
        private static readonly AttrWrapper<DateTime> End = new AttrWrapper<DateTime>("msdyn_end");
        private static readonly AttrWrapper<DateTime> Date = new AttrWrapper<DateTime>("msdyn_date");
        private static readonly AttrWrapper<string> TimeEntry = new AttrWrapper<string>("msdyn_timeentry");

        public void Execute(IServiceProvider serviceProvider)
        {
            // Obtain the tracing service
            var tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtain the execution context from the service provider.  
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            // The InputParameters collection contains all the data passed in the message request.  
            if (context.PrimaryEntityName == PrimaryEntityName
                && context.InputParameters.TryGetValue("Target", out var mayBeEntity)
                && mayBeEntity is Entity entity)
            {
                try
                {
                    ProcessFields(serviceProvider, entity, context);
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

        private static void ProcessFields(IServiceProvider serviceProvider, Entity entity, IPluginExecutionContext context)
        {
            if (Date.Get(entity) != default) // msdyn_date field is already used
            {
                return;
            }

            var msdynStart = Start.Get(entity);
            var msdynEnd = End.Get(entity);

            if (msdynEnd <= msdynStart)
            {
                throw new InvalidOperationException("Start date should be less than End date");
            }

            // Obtain the organization service reference which you will need for  
            // web service calls.  
            var serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            var service = serviceFactory.CreateOrganizationService(context.UserId);
            var existingTimeEntries = LoadExistingTimeEntries(service, msdynStart, msdynEnd);

            var n = 1;
            for (var date = msdynStart; date <= msdynEnd; date = date.AddDays(1), n++)
            {
                if (existingTimeEntries.Any(entry => Date.Get(entry) == date))
                {
                    continue;
                }
                var newTimeEntry = new Entity(context.PrimaryEntityName);
                Date.Set(newTimeEntry, date);
                TimeEntry.Set(newTimeEntry, TimeEntry.Get(entity) + " " + n);
                service.Create(newTimeEntry);
            }
        }

        private static Entity[] LoadExistingTimeEntries(IOrganizationService service, DateTime start, DateTime end)
        {
            var query = new QueryExpression
            {
                EntityName = PrimaryEntityName,
                ColumnSet = new ColumnSet(Date.Name),
                Criteria = new FilterExpression
                {
                    Filters =
                    {
                        new FilterExpression
                        {
                            FilterOperator = LogicalOperator.And,
                            Conditions =
                            {
                                new ConditionExpression
                                {
                                    AttributeName = Date.Name,
                                    Operator = ConditionOperator.OnOrAfter,
                                    Values = { start }
                                },
                                new ConditionExpression
                                {
                                    AttributeName = Date.Name,
                                    Operator = ConditionOperator.OnOrBefore,
                                    Values = { end }
                                }

                            }
                        }
                    }
                }
            };
            return service.RetrieveMultiple(query).Entities.ToArray();
        }
    }
}
