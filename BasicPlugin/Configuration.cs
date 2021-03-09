using System;

namespace BasicPlugin
{
    static class Configuration
    {
        public const string PrimaryEntityName = "msdyn_timeentry";
        public static readonly AttrWrapper<DateTime> Start = new AttrWrapper<DateTime>("msdyn_start");
        public static readonly AttrWrapper<DateTime> End = new AttrWrapper<DateTime>("msdyn_end");
        public static readonly AttrWrapper<DateTime> Date = new AttrWrapper<DateTime>("msdyn_date");
        public static readonly AttrWrapper<string> TimeEntry = new AttrWrapper<string>("msdyn_timeentry");
    }
}
