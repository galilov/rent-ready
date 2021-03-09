using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;
using static BasicPlugin.Configuration;

namespace BasicPlugin
{
    public class TimeEntryHandler
    {
        private readonly IOrganizationService _service;

        public TimeEntryHandler(IOrganizationService service)
        {
            _service = service;
        }

        public void PopulateFields(Entity entity)
        {
            if (Date.Get(entity) != default)
            {
                return;
            }

            var msdynStart = Start.Get(entity);
            var msdynEnd = End.Get(entity);

            if (msdynEnd <= msdynStart)
            {
                throw new InvalidOperationException("Start date should be less than End date");
            }

            var existingTimeEntries = LoadExistingTimeEntries(msdynStart, msdynEnd);

            var n = 1;
            for (var date = msdynStart; date <= msdynEnd; date = date.AddDays(1), n++)
            {
                if (existingTimeEntries.Any(entry => Date.Get(entry) == date))
                {
                    continue;
                }
                var newTimeEntry = new Entity(PrimaryEntityName);
                Date.Set(newTimeEntry, date);
                TimeEntry.Set(newTimeEntry, $"{TimeEntry.Get(entity)} {n}");
                _service.Create(newTimeEntry);
            }
        }

        private DataCollection<Entity> LoadExistingTimeEntries(DateTime start, DateTime end)
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
            return _service.RetrieveMultiple(query).Entities;
        }
    }
}
