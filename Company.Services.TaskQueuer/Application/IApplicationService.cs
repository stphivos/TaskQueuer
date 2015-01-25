using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Company.Services.TaskQueuer.Domain.Models;
using Company.Services.TaskQueuer.Domain.Seedwork;

namespace Company.Services.TaskQueuer.Application
{
    public interface IApplicationService
    {
        void Process<T>(DynamicDataQueuer<T> queuer, QueuerHandler<T> handler);
        void Process<T>(GivenDataQueuer<T> queuer, QueuerHandler<T> handler, IEnumerable<T> items);
    }
}