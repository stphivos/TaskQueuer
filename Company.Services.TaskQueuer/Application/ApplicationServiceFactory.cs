using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Company.Services.TaskQueuer.Infrastructure.CrossCutting;

namespace Company.Services.TaskQueuer.Application
{
    public class ApplicationServiceFactory
    {
        public IApplicationService Create()
        {
            var service = (ApplicationService)Config.Current.ApplicationServiceTypeName.GetLocalInstance();
            return service;
        }
    }
}