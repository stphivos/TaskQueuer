using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Company.Services.TaskQueuer.Domain.Seedwork;
using Company.Services.TaskQueuer.Domain.Models;
using Company.Services.TaskQueuer.Infrastructure.CrossCutting;

namespace Company.Services.TaskQueuer.Client.Handlers
{
    public class DynamicDataHandler : BaseHandler
    {
        #region Instances

        private readonly object _lock = new object();
        private readonly int[] _items = new int[] { 1, 2, 3, 4, 5 };
        private readonly SynchronizedCollection<int> _processed = new SynchronizedCollection<int>();

        #endregion

        #region Properties



        #endregion

        #region Methods

        #region Exposed

        public override void Execute()
        {
            this.Service.Process((DynamicDataQueuer<int>)this.Queuer, this.ExecuteHandler);
        }

        #endregion

        #region Handlers

        protected override void Queuer_ItemProcessing(object sender, QueuerItemEventArgs<int> e)
        {
            this.AssignNextItem(e);
            if (!e.Stop)
            {
                base.Queuer_ItemProcessing(sender, e);
            }
        }

        #endregion

        #region Helper

        protected override Queuer<T> GetQueuer<T>()
        {
            return new QueuerFactory().CreateDynamicDataQueuer<T>(parallelTaskCount: Config.Current.ParallelTaskCount, processItemTimeout: Config.Current.ProcessItemTimeout);
        }

        private void AssignNextItem(QueuerItemEventArgs<int> e)
        {
            lock (this._lock)
            {
                if (this._processed.Count == this._items.Length)
                {
                    e.Stop = true;
                }
                else
                {
                    int result = this._items.Where(x => !this._processed.Contains(x)).First();
                    this._processed.Add(e.Item = result);
                }
            }
        }

        #endregion

        #endregion
    }
}