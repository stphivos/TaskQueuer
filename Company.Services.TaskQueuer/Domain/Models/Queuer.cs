using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Company.Services.TaskQueuer.Domain.Seedwork;
using Company.Services.TaskQueuer.Domain.Services;
using Company.Services.TaskQueuer.Infrastructure.CrossCutting;

namespace Company.Services.TaskQueuer.Domain.Models
{
    public abstract class Queuer<T>
    {
        #region Instances

        private readonly object _taskProcessingLock = new object();
        
        #endregion

        #region Properties

        public bool IsProcessing { get; private set; }

        public int ParallelTaskCount { get; internal set; }
        public int ProcessItemTimeout { get; internal set; }

        internal SynchronizedCollection<T> ItemsInQueue { get; set; }
        internal SynchronizedCollection<T> ItemsCompleted { get; set; }

        private bool HasQueuedItems
        {
            get { return this.ItemsInQueue.Count > 0; }
        }

        private bool AreItemsProcessed
        {
            get { return this.ItemsCompleted.Count == this.ItemsInQueue.Count; }
        }

        #endregion

        #region Methods

        #region Constructors

        internal Queuer()
        {
            this.ItemsInQueue = new SynchronizedCollection<T>();
            this.ItemsCompleted = new SynchronizedCollection<T>();
        }

        #endregion

        #region Exposed


   
        #endregion

        #region Process

        protected virtual void Process_ExecuteAsync(QueuerState<T> state, int index)
        {
            var worker = new Thread(this.Process_ExecuteNext)
            {
                Name = string.Format("Worker {0}", index + 1)
            };
            worker.Start(state);
        }

        protected abstract void Process_ExecuteNext(object obj);

        protected virtual void Process_ExecuteSpecific(QueuerState<T> state)
        {
            var e = new QueuerItemEventArgs<T>(state.CurrentItem, Thread.CurrentThread.Name);

            // Before item process
            this.OnItemProcessing(e, state);

            // Item process
            if (!e.Stop)
            {
                this.Process_ExecuteHandler(state, e);
            }

            // After item process
            this.OnItemProcessed(new QueuerItemEventArgs<T>(e), state);

            // Next item
            if (!e.Stop)
            {
                this.Process_ExecuteNext(state);
            }
        }

        protected virtual void Process_ExecuteHandler(QueuerState<T> state, QueuerItemEventArgs<T> e)
        {
            try
            {
                using (var monitor = new QueuerMonitor(this.ProcessItemTimeout, string.Format("Process task {0}.", e.Item), e))
                {
                    monitor.TimeExceeded += new EventHandler<QueuerMonitorEventArgs>(Monitor_TimeExceeded);
                    state.Handler.Invoke(state.CurrentItem);
                }
            }
            catch (Exception ex)
            {
                e.Exception = e.Exception.Append(ex);
            }
            finally
            {
                if (e.Exception != null)
                {
                    this.OnItemError(new QueuerItemEventArgs<T>(e));
                }
            }
        }

        #region Item

        protected virtual void Item_MarkCompleted(QueuerState<T> state)
        {
            this.ItemsCompleted.Add(state.CurrentItem);
            state.CurrentItem = default(T);
        }

        protected virtual void Item_ProcessStopped(QueuerState<T> state)
        {
            var worker = new Thread(this.OnProcessFinalized);
            worker.Start(state);
        }

        protected virtual void Item_ProcessReset()
        {
            this.ItemsInQueue.Clear();
            this.ItemsCompleted.Clear();
            this.IsProcessing = false;
        }

        #endregion

        #endregion

        #region Handlers

        private void Monitor_TimeExceeded(object sender, QueuerMonitorEventArgs e)
        {
            QueuerItemEventArgs<T> args = (QueuerItemEventArgs<T>)(e.State);
            args.Exception = e.Exception;
        }

        #endregion

        #region Events

        public event EventHandler<QueuerProcessEventArgs<T>> ProcessInitialized;

        protected virtual void OnProcessInitialized(QueuerProcessEventArgs<T> e)
        {
            this.IsProcessing = true;

            EventHandler<QueuerProcessEventArgs<T>> handler = this.ProcessInitialized;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<QueuerProcessEventArgs<T>> ProcessFinalized;

        protected virtual void OnProcessFinalized(object obj)
        {
            lock (this._taskProcessingLock)
            {
                if (this.HasQueuedItems && this.AreItemsProcessed)
                {
                    this.Item_ProcessReset();

                    var stopTime = DateTime.Now;
                    var e = new QueuerProcessEventArgs<T>((QueuerState<T>)(obj), stopTime);

                    EventHandler<QueuerProcessEventArgs<T>> handler = this.ProcessFinalized;
                    if (handler != null) handler(this, e);
                }
            }
        }

        public event EventHandler<QueuerItemEventArgs<T>> ItemProcessing;

        protected virtual void OnItemProcessing(QueuerItemEventArgs<T> e, QueuerState<T> state)
        {
            EventHandler<QueuerItemEventArgs<T>> handler = this.ItemProcessing;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<QueuerItemEventArgs<T>> ItemProcessed;

        protected virtual void OnItemProcessed(QueuerItemEventArgs<T> e, QueuerState<T> state)
        {
            if (e.Stop)
            {
                this.Item_ProcessStopped(state);
            }
            else if (e.Exception == null)
            {
                EventHandler<QueuerItemEventArgs<T>> handler = this.ItemProcessed;
                if (handler != null) handler(this, e);
            }
        }

        public event EventHandler<QueuerItemEventArgs<T>> ItemError;

        protected virtual void OnItemError(QueuerItemEventArgs<T> e)
        {
            EventHandler<QueuerItemEventArgs<T>> handler = this.ItemError;
            if (handler != null) handler(this, e);
        }

        #endregion

        #endregion
    }
}