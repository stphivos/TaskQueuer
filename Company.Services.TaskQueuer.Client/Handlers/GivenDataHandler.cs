using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Company.Services.TaskQueuer.Domain.Models;
using Company.Services.TaskQueuer.Infrastructure.CrossCutting;

namespace Company.Services.TaskQueuer.Client.Handlers
{
    public class GivenDataHandler : BaseHandler
    {
        #region Instances



        #endregion

        #region Properties



        #endregion

        #region Methods

        #region Exposed

        public override void Execute()
        {
            this.Service.Process((GivenDataQueuer<int>)this.Queuer, this.ExecuteHandler, new int[] { 1, 2, 3, 4, 5 });
        }

        #endregion

        #region Helper

        protected override Queuer<T> GetQueuer<T>()
        {
            return new QueuerFactory().CreateGivenDataQueuer<T>(parallelTaskCount: Config.Current.ParallelTaskCount, processItemTimeout: Config.Current.ProcessItemTimeout);
        }

        #endregion

        #endregion
    }
}