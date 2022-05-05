using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

namespace io.github.thisisnozaku.idle.framework.Logging
{
    public class Logger
    {
        public Level LoggingLevel = Level.ERROR;
        private Dictionary<Level, Action<object>> Loggers;
        [ExcludeFromCoverage]
        public Logger() : this(new Dictionary<Level, Action<object>>() {
            { Level.DEBUG, obj => UnityEngine.Debug.Log(obj) },
            { Level.ERROR, obj => UnityEngine.Debug.LogError(obj) },
            { Level.INFO, obj => UnityEngine.Debug.Log(obj) },
            { Level.TRACE, obj => UnityEngine.Debug.Log(obj) },
            { Level.WARN, obj => UnityEngine.Debug.LogWarning(obj) },
        })
        {
        }

        public Logger(Dictionary<Level, Action<object>> Loggers)
        {
            this.Loggers = Loggers;
        }

        public void Log(Level level, object toLog)
        {
            if (level <= LoggingLevel)
            {
                Loggers[level](toLog);
            }
        }

        public void Warning(object toLog)
        {
            Log(Level.WARN, toLog);
        }

        public void Error(object toLog)
        {
            Log(Level.ERROR, toLog);
        }

        public void Info(object toLog)
        {
            Log(Level.INFO, toLog);
        }

        public void Debug(object toLog)
        {
            Log(Level.DEBUG, toLog);
        }

        public void Trace(object toLog)
        {
            Log(Level.TRACE, toLog);
        }

        public static Logger GetLogger()
        {
            return new Logger();
        }

        public enum Level
        {
            ERROR,
            WARN,
            INFO,
            DEBUG,
            TRACE
        }
    }
}