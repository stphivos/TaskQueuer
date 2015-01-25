using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Company.Services.TaskQueuer.Domain.Models
{
    public class QueuerFactory
    {
        public Queuer<T> CreateGivenDataQueuer<T>(int parallelTaskCount, int processItemTimeout)
        {
            var queuer = new GivenDataQueuer<T>()
            {
                ParallelTaskCount = parallelTaskCount,
                ProcessItemTimeout = processItemTimeout
            };
            return queuer;
        }

        public Queuer<T> CreateDynamicDataQueuer<T>(int parallelTaskCount, int processItemTimeout)
        {
            var queuer = new DynamicDataQueuer<T>()
            {
                ParallelTaskCount = parallelTaskCount,
                ProcessItemTimeout = processItemTimeout
            };
            return queuer;
        }
    }
}