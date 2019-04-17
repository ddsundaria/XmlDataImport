using log4net;
using Sitecore.Diagnostics;
using Sitecore.Services.Core.Diagnostics;
using System.Globalization;
using System;

namespace SaudiAramco.Foundation.XmlDataImport.Loggers
{
    public class Logger : ILogger
    {
        public Logger()
        {
            Log = LoggerFactory.GetLogger("SaudiAramco.DXF.Logger");
        }
        public string Prefix { get; set; }
        public ILog Log { get; set; }

        public void Debug(string message)
        {
            if (Log.IsDebugEnabled)
                Log.Debug(Format(message, "DEBUG"));
        }

        public void Debug(string messageFormat, params object[] args)
        {
            Debug(string.Format(CultureInfo.InvariantCulture, messageFormat, args));
        }

        public void Error(string message)
        {
            if (Log.IsErrorEnabled)
                Log.Error(Format(message, "ERROR"));
        }

        public void Error(string messageFormat, params object[] args)
        {
            Error(string.Format(CultureInfo.InvariantCulture, messageFormat, args));
        }

        public void Fatal(string message)
        {
            if (Log.IsFatalEnabled)
                Log.Fatal(Format(message, "FATAL"));
        }

        public void Fatal(string messageFormat, params object[] args)
        {
            Fatal(string.Format(CultureInfo.InvariantCulture, messageFormat, args));
        }

        public void Info(string message)
        {
            if (Log.IsInfoEnabled)
                Log.Info(Format(message, "INFO"));
        }

        public void Info(string messageFormat, params object[] args)
        {
            Info(string.Format(CultureInfo.InvariantCulture, messageFormat, args));
        }

        public void Warn(string message)
        {
            if (Log.IsWarnEnabled)
                Log.Warn(Format(message, "WARN"));
        }

        public void Warn(string messageFormat, params object[] args)
        {
            Warn(string.Format(CultureInfo.InvariantCulture, messageFormat, args));
        }

        private static string Format(string message, string prefix)
        {
            return string.Format(CultureInfo.InvariantCulture, "[{0}] {1}", prefix, message);
        }
    }
}
