using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Company.Services.TaskQueuer.Domain.Seedwork;
using Company.Services.TaskQueuer.Infrastructure.CrossCutting;

namespace Company.Services.TaskQueuer.Domain.Models
{
    public class GivenDataQueuer<T> : Queuer<T>
    {
        #region Instances

        private readonly object _taskQueryLock = new object();

        #endregion

        #region Properties



        #endregion

        #region Methods

        #region Exposed

        internal void Process(QueuerHandler<T> handler, IEnumerable<T> items)
        {
            var state = new QueuerState<T>(handler, items);
            var args = new QueuerProcessEventArgs<T>(state);
            var count = Math.Min(this.ParallelTaskCount, items.Count());

            this.OnProcessInitialized(args);

            for (int i = 0; i < count; i++)
            {
                this.Process_ExecuteAsync(state, i);
            }
        }

        #endregion

        #region Process

        protected override void Process_ExecuteNext(object obj)
        {
            var state = new QueuerState<T>((QueuerState<T>)(obj));

            if (this.Process_AssignNext(state))
            {
                this.Process_ExecuteSpecific(state);
            }
            else
            {
                this.OnProcessFinalized(state);
            }
        }

        private bool Process_AssignNext(QueuerState<T> state)
        {
            lock (this._taskQueryLock)
            {
                T nextTask = this.IsProcessing ? state.Items.Where(x => !this.ItemsInQueue.Contains(x)).FirstOrDefault() : default(T);

                if (nextTask.IsDefaultValue())
                {
                    return false;
                }
                else
                {
                    this.ItemsInQueue.Add(state.CurrentItem = nextTask);
                    return true;
                }
            }
        }

        #endregion

        #region Events

        protected override void OnItemProcessed(QueuerItemEventArgs<T> e, QueuerState<T> state)
        {
            this.Item_MarkCompleted(state);
            base.OnItemProcessed(e, state);
        }

        #endregion

        #endregion
    }
}