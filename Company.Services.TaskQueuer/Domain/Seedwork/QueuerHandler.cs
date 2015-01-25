using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Company.Services.TaskQueuer.Domain.Seedwork
{
    public delegate void QueuerHandler<T>(T state);
}