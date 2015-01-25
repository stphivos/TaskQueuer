using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Company.Services.TaskQueuer.Domain.Services
{
    internal class QueuerMonitorEventArgs : EventArgs
    {
        public object State { get; private set; }
        public Exception Exception { get; private set; }

        public QueuerMonitorEventArgs(object state, Exception exception)
        {
            this.State = state;
            this.Exception = exception;
        }
    }
}