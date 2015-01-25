using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Company.Services.TaskQueuer.Domain.Seedwork
{
    public class QueuerProcessEventArgs<T> : BaseEventArgs
    {
        public QueuerState<T> State { get; private set; }
        public DateTime? StopTime { get; private set; }

        public TimeSpan? Duration
        {
            get
            {
                TimeSpan? result = null;
                if (this.StopTime.HasValue)
                {
                    result = this.StopTime.Value - this.State.StartTime;
                }
                return result;
            }
        }

        public QueuerProcessEventArgs(QueuerState<T> state)
        {
            this.State = state;
        }

        public QueuerProcessEventArgs(QueuerState<T> state, DateTime stopTime) : this(state)
        {
            this.StopTime = stopTime;
        }
    }
}