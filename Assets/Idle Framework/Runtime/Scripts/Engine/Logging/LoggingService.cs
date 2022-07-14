using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Engine.Logging
{
    public class LoggingService
    {
        private Dictionary<string, Dictionary<LogType, bool>> LoggingContextLevels = new Dictionary<string, Dictionary<LogType, bool>>()
        {
            { "*", new Dictionary<LogType, bool>() { { LogType.Error, true} } }
        };

        public void Log(LogType logType, Func<string> logMessageGenerator, string logContext = null)
        {
            bool logEnabled = false;
            logContext = logContext != null ? logContext : "*";
            if (LoggingContextLevels.ContainsKey(logContext))
            {
                LoggingContextLevels[logContext].TryGetValue(logType, out logEnabled);
            }
            else
            {
                LoggingContextLevels["*"].TryGetValue(logType, out logEnabled);
            }
            if (logEnabled)
            {
                Log(logType, logMessageGenerator(), logContext);
            }
        }

        public void Log(LogType logType, string logMessage, string logContext = null)
        {
            bool logEnabled = false;
            logContext = logContext != null ? logContext : "*";
            if (LoggingContextLevels.ContainsKey(logContext))
            {
                LoggingContextLevels[logContext].TryGetValue(logType, out logEnabled);
            }
            else
            {
                LoggingContextLevels["*"].TryGetValue(logType, out logEnabled);
            }
            if (logEnabled)
            {
                string finalMessage = string.Format("[{0}] {1}", logContext, logMessage);
                switch (logType)
                {
                    case LogType.Error:
                    case LogType.Exception:
                        Debug.LogError(finalMessage);
                        break;
                    case LogType.Log:
                        Debug.Log(finalMessage);
                        break;
                    case LogType.Warning:
                        Debug.LogWarning(finalMessage);
                        break;
                }
            }
        }

        public void ConfigureLogging(string logContext, LogType? logLevel, bool enabled = true)
        {
            Dictionary<LogType, bool> contexts;
            switch (logLevel)
            {
                case LogType.Log:
                    ConfigureLogging(logContext, LogType.Warning, enabled);
                    break;
                case LogType.Warning:
                    ConfigureLogging(logContext, LogType.Error, enabled);
                    break;
            }
            if (!LoggingContextLevels.TryGetValue(logContext, out contexts))
            {
                contexts = new Dictionary<LogType, bool>();
                LoggingContextLevels[logContext] = contexts;
            }
            if (!logLevel.HasValue)
            {
                contexts.Clear();
            }
            else
            {
                contexts[logLevel.Value] = enabled;
            }
        }
    }
}