using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Company.Services.TaskQueuer.Infrastructure.CrossCutting
{
    public class Config
    {
        #region Properties

        // Static
        public static Config Current { get; set; }

        // Application
        public string ApplicationServiceTypeName { get; set; }

        // Logging
        public string LoggerTypeName { get; set; }
        public string EventSource { get; set; }

        // Queuer
        public int ParallelTaskCount { get; set; }
        public int ProcessItemTimeout { get; set; }

        #endregion

        #region Methods

        #region Constructors

        private Config()
        {
        }

        #endregion

        #region Exposed

        public static Config Parse(string[] args)
        {
            var config = Config.GetInstanceFromAppConfig().GetOverwritesFrom(Config.GetInstanceFromArgs(args));
            return config;
        }

        #endregion

        #region Helper

        private static Config GetInstanceFromAppConfig()
        {
            var config = new Config()
            {
                ApplicationServiceTypeName = Config.GetKeyValue<string>("ApplicationServiceTypeName"),
                LoggerTypeName = Config.GetKeyValue<string>("LoggerTypeName"),
                EventSource = Config.GetKeyValue<string>("EventSource"),
                ParallelTaskCount = Config.GetKeyValue<int>("ParallelTaskCount", 4),
                ProcessItemTimeout = Config.GetKeyValue<int>("ProcessItemTimeout")
            };
            return config;
        }

        private static T GetKeyValue<T>(string key, T valueIfNotFound)
        {
            T value = Config.GetKeyValue<T>(key, false);
            if (value.IsDefaultValue())
            {
                return valueIfNotFound;
            }
            else
            {
                return value;
            }
        }

        private static T GetKeyValue<T>(string key, bool isRequired = true)
        {
            var typedValue = default(T);

            var value = ConfigurationManager.AppSettings[key];
            if (value != null)
            {
                typedValue = value.ConvertTo<T>();
            }
            else if (isRequired)
            {
                throw new Exception(string.Format("Required key '{0}' not found in configuration.", key));
            }

            return typedValue;
        }

        private static Config GetInstanceFromArgs(string[] args)
        {
            Config config = new Config();

            foreach (var arg in args)
            {
                var parts = arg.Split('=');
                var key = parts.First();
                var value = parts.Skip(1).First();

                switch (key.ToLower())
                {
                    case "a":
                    case "applicationservicetypename":
                        config.ApplicationServiceTypeName = value;
                        break;
                    case "l":
                    case "loggertypename":
                        config.LoggerTypeName = value;
                        break;
                    case "s":
                    case "eventsource":
                        config.EventSource = value;
                        break;
                    case "c":
                    case "paralleltaskcount":
                        config.ParallelTaskCount = value.ConvertTo<int>();
                        break;
                    case "t":
                    case "processitemtimeout":
                        config.ProcessItemTimeout = value.ConvertTo<int>();
                        break;
                }
            }

            return config;
        }

        private Config GetOverwritesFrom(Config source)
        {
            var config = new Config()
            {
                ApplicationServiceTypeName = source.ApplicationServiceTypeName.IsDefaultValue() ? this.ApplicationServiceTypeName : source.ApplicationServiceTypeName,
                LoggerTypeName = source.LoggerTypeName.IsDefaultValue() ? this.LoggerTypeName : source.LoggerTypeName,
                EventSource = source.EventSource.IsDefaultValue() ? this.EventSource : source.EventSource,
                ParallelTaskCount = source.ParallelTaskCount.IsDefaultValue() ? this.ParallelTaskCount : source.ParallelTaskCount,
                ProcessItemTimeout = source.ProcessItemTimeout.IsDefaultValue() ? this.ProcessItemTimeout : source.ProcessItemTimeout
            };
            return config;
        }

        #endregion

        #endregion
    }
}