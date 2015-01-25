using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Company.Services.TaskQueuer.Domain.Seedwork;

namespace Company.Services.TaskQueuer.Domain.Models
{
    public class DynamicDataQueuer<T> : Queuer<T>
    {
        #region Instances



        #endregion

        #region Properties



        #endregion

        #region Methods

        #region Exposed

        internal void Process(QueuerHandler<T> handler)
        {
            var state = new QueuerState<T>(handler);
            var args = new QueuerProcessEventArgs<T>(state);

            this.OnProcessInitialized(args);

            for (int i = 0; i < this.ParallelTaskCount; i++)
            {
                this.Process_ExecuteAsync(state, i);
            }
        }

        #endregion

        #region Process

        protected override void Process_ExecuteNext(object obj)
        {
            var state = new QueuerState<T>((QueuerState<T>)(obj));
            this.Process_ExecuteSpecific(state);
        }

        #endregion

        #region Events

        protected override void OnItemProcessing(QueuerItemEventArgs<T> e, QueuerState<T> state)
        {
            base.OnItemProcessing(e, state);
            if (!e.Stop)
            {
                this.ItemsInQueue.Add(state.CurrentItem = e.Item);
            }
        }

        protected override void OnItemProcessed(QueuerItemEventArgs<T> e, QueuerState<T> state)
        {
            if (!e.Stop)
            {
                this.Item_MarkCompleted(state);
            }
            base.OnItemProcessed(e, state);
        }

        #endregion

        #endregion
    }
}