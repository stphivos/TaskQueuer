using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Company.Services.TaskQueuer.Domain.Models;
using Company.Services.TaskQueuer.Application;
using Company.Services.TaskQueuer.Domain.Seedwork;
using System.Threading;
using Company.Services.TaskQueuer.Client.Handlers;
using Company.Services.TaskQueuer.Infrastructure.CrossCutting;

namespace Company.Services.TaskQueuer.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Config.Current = Config.Parse(args);
            var handler = new GivenDataHandler();
            
            try
            {
                handler.Execute();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                while (!handler.IsFinalized)
                {
                    Thread.Sleep(100);
                }

                foreach (var item in handler.Events.OrderBy(x => x.Key.Timestamp))
                    Console.WriteLine(item.Value);

                Console.Read();
            }
        }
    }
}