using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Company.Services.TaskQueuer.Infrastructure.CrossCutting;

namespace Company.Services.TaskQueuer.Infrastructure.Logging
{
    public class WindowsEventLogger : Logger
    {
        public override void WriteEntry(string title, string message, EventLogEntryType type)
        {
            EventLog.WriteEntry(Config.Current.EventSource, message, type);
        }
    }
}