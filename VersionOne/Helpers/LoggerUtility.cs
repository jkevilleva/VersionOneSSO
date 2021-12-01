using log4net;
using log4net.Appender;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace VersionOne.Helpers
{
    public static class LoggerUtility
    {
        private static ILog _log = null;
        private static string _logFile = null;
        public enum TracingLevel
        {
            ALL, DEBUG, INFO, WARN, ERROR, FATAL, OFF
        }
      

       
        public static void LogMessage(TracingLevel Level, string Message,Exception ex)
        {
            switch (Level)
            {
                case TracingLevel.DEBUG:
                    _log.Debug(Message);
                    break;

                case TracingLevel.INFO:
                    _log.Info(Message);
                    break;

                case TracingLevel.WARN:
                    _log.Warn(Message);
                    break;

                case TracingLevel.ERROR:
                    _log.Error(Message,ex);
                    break;

                case TracingLevel.FATAL:
                    _log.Fatal(Message, ex);
                    break;
            }
        }

       
        public static void Initialize(string ApplicationPath)
        {
            _logFile = Path.Combine(ApplicationPath, "App_Data", "VersionOne.log");
            GlobalContext.Properties["LogFileName"] = _logFile;

            log4net.Config.XmlConfigurator.Configure(new FileInfo(Path.Combine(ApplicationPath, "Log4Net.config")));

            _log = LogManager.GetLogger("VersionOne");

        }
        public static string LogFile
        {
            get { return _logFile; }
        }
    }
   
    
}