using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Company.Services.TaskQueuer.Domain.Models;
using Company.Services.TaskQueuer.Domain.Seedwork;
using Company.Services.TaskQueuer.Infrastructure.Logging;

namespace Company.Services.TaskQueuer.Application
{
    internal class ApplicationService : IApplicationService
    {
        #region Instances

        private readonly Logger Logger = new LoggerFactory().Create();

        #endregion

        #region Methods

        #region Exposed

        public void Process<T>(DynamicDataQueuer<T> queuer, QueuerHandler<T> handler)
        {
            try
            {
                queuer.Process(handler);
            }
            catch (Exception ex)
            {
                this.Logger.WriteException(ex);
                throw;
            }
        }

        public void Process<T>(GivenDataQueuer<T> queuer, QueuerHandler<T> handler, IEnumerable<T> items)
        {
            try
            {
                queuer.Process(handler, items);
            }
            catch (Exception ex)
            {
                this.Logger.WriteException(ex);
                throw;
            }
        }

        #endregion

        #endregion
    }
}