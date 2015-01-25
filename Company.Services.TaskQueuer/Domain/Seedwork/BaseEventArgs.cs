using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Company.Services.TaskQueuer.Domain.Seedwork
{
    public abstract class BaseEventArgs : EventArgs
    {
        public DateTime Timestamp { get; internal set; }

        public BaseEventArgs()
        {
            this.Timestamp = DateTime.Now;
        }
    }
}