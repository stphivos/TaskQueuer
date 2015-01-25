using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Company.Services.TaskQueuer.Domain.Services
{
    internal class QueuerMonitor : IDisposable
    {
        #region Constants

        private const int POLL_INTERVAL = 100;

        #endregion

        #region Instances

        private bool _isDisposing;

        #endregion

        #region Properties

        public object State { get; private set; }
        public int Timeout { get; private set; }
        public string Action { get; private set; }

        #endregion

        #region Methods

        #region Constructors

        public QueuerMonitor(int timeout, string action) : this(timeout, action, null)
        {
        }

        public QueuerMonitor(int timeout, string action, object state)
        {
            this.Timeout = timeout;
            this.Action = action;
            this.State = state;
            this.TrackAsync(state);
        }

        #endregion

        #region Exposed

        public void Dispose()
        {
            this._isDisposing = true;
        }

        #endregion

        #region Tracking

        private void TrackAsync(object state)
        {
            Thread worker = new Thread(this.Track);
            worker.Start(state);
        }

        private void Track(object state)
        {
            int wait = 0;
            while (!this._isDisposing)
            {
                if (wait >= this.Timeout)
                {
                    string time = Math.Round((double)this.Timeout / (double)1000, 2).ToString();
                    Exception ex = new Exception(string.Format("Action '{0}' has timed out after {1} seconds.", this.Action, time));
                    this.OnTimeExceeded(new QueuerMonitorEventArgs(state, ex));
                    break;
                }
                else
                {
                    Thread.Sleep(POLL_INTERVAL);
                    wait += POLL_INTERVAL;
                }
            }
        }

        #endregion

        #region Events

        public event EventHandler<QueuerMonitorEventArgs> TimeExceeded;

        protected virtual void OnTimeExceeded(QueuerMonitorEventArgs e)
        {
            EventHandler<QueuerMonitorEventArgs> handler = this.TimeExceeded;
            if (handler != null) handler(this, e);
        }

        #endregion

        #endregion
    }
}