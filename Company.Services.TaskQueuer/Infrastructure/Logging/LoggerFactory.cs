using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Company.Services.TaskQueuer.Infrastructure.CrossCutting;

namespace Company.Services.TaskQueuer.Infrastructure.Logging
{
    public class LoggerFactory
    {
        public Logger Create()
        {
            var logger = (Logger)Config.Current.LoggerTypeName.GetLocalInstance();
            return logger;
        }
    }
}