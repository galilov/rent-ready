using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using BasicPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicPlugin.Tests
{
    [TestClass()]
    public class MyPlugiTimeEntryHandlerTests
    {
        [TestMethod()]
        public void TestPopulateFieldsWithConflictingEntries()
        {

            var entry_01_mar_2021 = new Entity("msdyn_timeentry");
            entry_01_mar_2021.Attributes["msdyn_date"] = DateTime.Parse("2021-03-01T20:00:00.0000000Z");

            var entry_02_mar_2021 = new Entity("msdyn_timeentry");
            entry_02_mar_2021.Attributes["msdyn_date"] = DateTime.Parse("2021-03-02T20:00:00.0000000Z");

            var entry_05_mar_2021 = new Entity("msdyn_timeentry");
            entry_05_mar_2021.Attributes["msdyn_date"] = DateTime.Parse("2021-03-05T20:00:00.0000000Z");

            var entry_06_mar_2021 = new Entity("msdyn_timeentry");
            entry_06_mar_2021.Attributes["msdyn_date"] = DateTime.Parse("2021-03-06T20:00:00.0000000Z");

            var entry_03_mar_2021 = new Entity("msdyn_timeentry");
            entry_03_mar_2021.Attributes["msdyn_date"] = DateTime.Parse("2021-03-03T20:00:00.0000000Z");

            var entry_04_mar_2021 = new Entity("msdyn_timeentry");
            entry_04_mar_2021.Attributes["msdyn_date"] = DateTime.Parse("2021-03-04T20:00:00.0000000Z");

            var entry_07_mar_2021 = new Entity("msdyn_timeentry");
            entry_07_mar_2021.Attributes["msdyn_date"] = DateTime.Parse("2021-03-07T20:00:00.0000000Z");

            var entry_01_07_mar_2021 = new Entity("msdyn_timeentry");
            entry_01_07_mar_2021.Attributes["msdyn_start"] = DateTime.Parse("2021-03-01T20:00:00.0000000Z");
            entry_01_07_mar_2021.Attributes["msdyn_end"] = DateTime.Parse("2021-03-07T20:00:00.0000000Z");

            var organizationService = new OrganizationService();

            organizationService.AddToConflictingEntity(entry_03_mar_2021);
            organizationService.AddToConflictingEntity(entry_04_mar_2021);
            organizationService.AddToConflictingEntity(entry_07_mar_2021);

            var timeEntryHandler = new TimeEntryHandler(organizationService);
            timeEntryHandler.PopulateFields(entry_01_07_mar_2021);

            Assert.AreEqual(4, organizationService.GetCreatedEntities().Count);

            Entity[] expectedDates = { entry_01_mar_2021, entry_02_mar_2021, entry_05_mar_2021, entry_06_mar_2021 };
            int i = 0;
            foreach(var entity in organizationService.GetCreatedEntities())
            {
                Assert.AreEqual(expectedDates[i].Attributes["msdyn_date"], entity.Attributes["msdyn_date"]);
                i++;
            }
        }

        [TestMethod()]
        public void TestPopulateFieldsWithoutConflictingEntries()
        {
            var entry_01_mar_2021 = new Entity("msdyn_timeentry");
            entry_01_mar_2021.Attributes["msdyn_date"] = DateTime.Parse("2021-03-01T20:00:00.0000000Z");

            var entry_02_mar_2021 = new Entity("msdyn_timeentry");
            entry_02_mar_2021.Attributes["msdyn_date"] = DateTime.Parse("2021-03-02T20:00:00.0000000Z");

            var entry_03_mar_2021 = new Entity("msdyn_timeentry");
            entry_03_mar_2021.Attributes["msdyn_date"] = DateTime.Parse("2021-03-03T20:00:00.0000000Z");

            var entry_04_mar_2021 = new Entity("msdyn_timeentry");
            entry_04_mar_2021.Attributes["msdyn_date"] = DateTime.Parse("2021-03-04T20:00:00.0000000Z");

            var entry_05_mar_2021 = new Entity("msdyn_timeentry");
            entry_05_mar_2021.Attributes["msdyn_date"] = DateTime.Parse("2021-03-05T20:00:00.0000000Z");

            var entry_06_mar_2021 = new Entity("msdyn_timeentry");
            entry_06_mar_2021.Attributes["msdyn_date"] = DateTime.Parse("2021-03-06T20:00:00.0000000Z");

            var entry_07_mar_2021 = new Entity("msdyn_timeentry");
            entry_07_mar_2021.Attributes["msdyn_date"] = DateTime.Parse("2021-03-07T20:00:00.0000000Z");

            var entry_01_07_mar_2021 = new Entity("msdyn_timeentry");
            entry_01_07_mar_2021.Attributes["msdyn_start"] = DateTime.Parse("2021-03-01T20:00:00.0000000Z");
            entry_01_07_mar_2021.Attributes["msdyn_end"] = DateTime.Parse("2021-03-07T20:00:00.0000000Z");

            var organizationService = new OrganizationService();

            var timeEntryHandler = new TimeEntryHandler(organizationService);
            timeEntryHandler.PopulateFields(entry_01_07_mar_2021);

            Assert.AreEqual(7, organizationService.GetCreatedEntities().Count);

            Entity[] expectedDates = { entry_01_mar_2021, entry_02_mar_2021, entry_03_mar_2021, 
                entry_04_mar_2021, entry_05_mar_2021, entry_06_mar_2021, entry_07_mar_2021, };
            int i = 0;
            foreach (var entity in organizationService.GetCreatedEntities())
            {
                Assert.AreEqual(expectedDates[i].Attributes["msdyn_date"], entity.Attributes["msdyn_date"]);
                i++;
            }

        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException), "Start date should be less than End date")]
        public void TestPopulateFieldsWrongDateRange1()
        {
            var entry_07_01_mar_2021 = new Entity("msdyn_timeentry");
            entry_07_01_mar_2021.Attributes["msdyn_start"] = DateTime.Parse("2021-03-07T20:00:00.0000000Z");
            entry_07_01_mar_2021.Attributes["msdyn_end"] = DateTime.Parse("2021-03-01T20:00:00.0000000Z");

            var organizationService = new OrganizationService();

            var timeEntryHandler = new TimeEntryHandler(organizationService);
            timeEntryHandler.PopulateFields(entry_07_01_mar_2021);

            Assert.Fail();
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException), "Start date should be less than End date")]
        public void TestPopulateFieldsWrongDateRange2()
        {
            var entry_02_01_mar_2021 = new Entity("msdyn_timeentry");
            entry_02_01_mar_2021.Attributes["msdyn_start"] = DateTime.Parse("2021-03-02T20:00:00.0000000Z");
            entry_02_01_mar_2021.Attributes["msdyn_end"] = DateTime.Parse("2021-03-01T20:00:00.0000000Z");

            var organizationService = new OrganizationService();

            var timeEntryHandler = new TimeEntryHandler(organizationService);
            timeEntryHandler.PopulateFields(entry_02_01_mar_2021);

            Assert.Fail();
        }

        [TestMethod()]
        public void TestPopulateFieldsShouldSkipHandling()
        {
            var entry_01_mar_2021 = new Entity("msdyn_timeentry");
            entry_01_mar_2021.Attributes["msdyn_date"] = DateTime.Parse("2021-03-01T20:00:00.0000000Z");

            var organizationService = new OrganizationService();

            var timeEntryHandler = new TimeEntryHandler(organizationService);
            timeEntryHandler.PopulateFields(entry_01_mar_2021);

            Assert.AreEqual(0, organizationService.GetCreatedEntities().Count);
        }
    }
}