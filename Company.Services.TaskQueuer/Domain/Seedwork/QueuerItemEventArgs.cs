using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Company.Services.TaskQueuer.Domain.Seedwork
{
    public class QueuerItemEventArgs<T> : BaseEventArgs
    {
        public T Item { get; set; }
        public bool Stop { get; set; }

        public Exception Exception { get; internal set; }
        public string Worker { get; private set; }

        public QueuerItemEventArgs(T item, string worker)
        {
            this.Item = item;
            this.Worker = worker;
        }

        public QueuerItemEventArgs(QueuerItemEventArgs<T> args) : this(args.Item, args.Worker)
        {
            this.Exception = args.Exception;
            this.Stop = args.Stop;
        }
    }
}