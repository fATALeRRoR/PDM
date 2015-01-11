using log4net;
using log4net.Config;

namespace PDM.Client.Helpers
{
    public static class Logging
    {
        private static readonly ILog _Log = LogManager.GetLogger("GlobalLogger");

        static Logging()
        {
            XmlConfigurator.Configure();
        }

        #region Public methods

        public static void Debug(object message)
        {
            _Log.Debug(message);
        }

        public static void Debug(string format, params object[] args)
        {
            _Log.DebugFormat(format, args);
        }

        public static void Info(object message)
        {
            _Log.Info(message);
        }

        public static void Info(string format, params object[] args)
        {
            _Log.InfoFormat(format, args);
        }

        public static void Warning(object message)
        {
            _Log.Warn(message);
        }

        public static void Warning(string format, params object[] args)
        {
            _Log.WarnFormat(format, args);
        }

        public static void Error(object message)
        {
            _Log.Error(message);
        }

        public static void Error(string format, params object[] args)
        {
            _Log.ErrorFormat(format, args);
        }

        #endregion
    }
}