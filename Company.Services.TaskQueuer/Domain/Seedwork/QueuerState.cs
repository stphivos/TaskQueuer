using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Company.Services.TaskQueuer.Domain.Seedwork
{
    public class QueuerState<T>
    {
        public T CurrentItem { get; set; }

        public DateTime StartTime { get; private set; }
        public QueuerHandler<T> Handler { get; private set; }
        public IEnumerable<T> Items { get; private set; }

        public QueuerState(QueuerHandler<T> handler)
        {
            this.Handler = handler;
            this.StartTime = DateTime.Now;
        }

        public QueuerState(QueuerHandler<T> handler, IEnumerable<T> items) : this(handler)
        {
            this.Items = items;
        }

        public QueuerState(QueuerState<T> state) : this(state.Handler, state.Items)
        {
        }
    }
}