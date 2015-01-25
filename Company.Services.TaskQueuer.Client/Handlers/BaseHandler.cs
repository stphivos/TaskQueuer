using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Company.Services.TaskQueuer.Application;
using Company.Services.TaskQueuer.Domain.Models;
using Company.Services.TaskQueuer.Domain.Seedwork;

namespace Company.Services.TaskQueuer.Client.Handlers
{
    public abstract class BaseHandler
    {
        #region Instances



        #endregion

        #region Properties

        public bool IsFinalized { get; private set; }
        public IApplicationService Service { get; private set; }
        public Queuer<int> Queuer { get; private set; }
        public Dictionary<BaseEventArgs, string> Events { get; private set; }

        #endregion

        #region Methods

        #region Constructors

        public BaseHandler()
        {
            this.Service = new ApplicationServiceFactory().Create();

            this.Queuer = this.GetQueuer<int>();
            this.Queuer.ItemProcessing += new EventHandler<Domain.Seedwork.QueuerItemEventArgs<int>>(Queuer_ItemProcessing);
            this.Queuer.ItemProcessed += new EventHandler<Domain.Seedwork.QueuerItemEventArgs<int>>(Queuer_ItemProcessed);
            this.Queuer.ItemError += new EventHandler<Domain.Seedwork.QueuerItemEventArgs<int>>(Queuer_ItemError);
            this.Queuer.ProcessInitialized += new EventHandler<Domain.Seedwork.QueuerProcessEventArgs<int>>(Queuer_ProcessInitialized);
            this.Queuer.ProcessFinalized += new EventHandler<Domain.Seedwork.QueuerProcessEventArgs<int>>(Queuer_ProcessFinalized);

            this.Events = new Dictionary<BaseEventArgs, string>();
        }

        #endregion

        #region Exposed
        
        public abstract void Execute();

        #endregion

        #region Handlers

        protected virtual void Queuer_ItemProcessing(object sender, Domain.Seedwork.QueuerItemEventArgs<int> e)
        {
            this.Events.Add(e, string.Format("Started item '{0}' ({1}).", e.Item, e.Worker));
        }

        protected virtual void Queuer_ItemProcessed(object sender, Domain.Seedwork.QueuerItemEventArgs<int> e)
        {
            this.Events.Add(e, string.Format("Completed item '{0}' ({1}).", e.Item, e.Worker));
        }

        protected virtual void Queuer_ItemError(object sender, Domain.Seedwork.QueuerItemEventArgs<int> e)
        {
            this.Events.Add(e, string.Format("Error item '{0}' ({1}) -> {2}.", e.Item, e.Worker, e.Exception.Message));
        }

        protected virtual void Queuer_ProcessInitialized(object sender, Domain.Seedwork.QueuerProcessEventArgs<int> e)
        {
            this.Events.Add(e, string.Format("Process started at {0}.", e.State.StartTime));
        }

        protected virtual void Queuer_ProcessFinalized(object sender, Domain.Seedwork.QueuerProcessEventArgs<int> e)
        {
            this.Events.Add(e, string.Format("Process ended at {0} (Duration: {1}).", e.StopTime, e.Duration));
            this.IsFinalized = true;
        }

        #endregion

        #region Helper

        protected abstract Queuer<T> GetQueuer<T>();

        protected virtual void ExecuteHandler(int data)
        {
            switch (data)
            {
                case 1:
                    throw new Exception("Error 1");
                //break;
                case 2:
                    Thread.Sleep(1500);
                    break;
                case 3:
                    Thread.Sleep(1500);
                    throw new Exception("Error 3");
                //break;
                case 4:
                    for (int i = 0; i < 10000; i++) { }
                    break;
                case 5:
                    for (int i = 0; i < 100000; i++) { }
                    break;
            }
        }

        #endregion

        #endregion
    }
}