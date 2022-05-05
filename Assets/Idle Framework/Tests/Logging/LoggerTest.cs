using io.github.thisisnozaku.idle.framework.Logging;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using static io.github.thisisnozaku.idle.framework.Logging.Logger;

namespace io.github.thisisnozaku.idle.framework.Tests
{
    public class LoggerTest
    {
        private Logger Logger;
        private Dictionary<Logger.Level, Action<object>> LoggingMethods;
        private Dictionary<Logger.Level, bool> Called;
        [SetUp]
        public void setup()
        {
            Called = new Dictionary<Level, bool>();
            LoggingMethods = new Dictionary<Level, Action<object>>() {
            { Level.DEBUG, obj => Called[Level.DEBUG] = true },
            { Level.ERROR, obj => Called[Level.ERROR] = true },
            { Level.INFO, obj => Called[Level.INFO] = true },
            { Level.TRACE, obj => Called[Level.TRACE] = true },
            { Level.WARN, obj => Called[Level.WARN] = true }
        };
            Logger = new Logger(LoggingMethods);
        }
        [Test]
        public void DoesNotLogIfLevelBelowConfiguredLevel()
        {

            Logger.Error("log");
            Logger.Trace("log");
            Logger.Info("log");
            Logger.Debug("log");
            Logger.Warning("log");
            Assert.True(Called.ContainsKey(Level.ERROR));
            Assert.False(Called.ContainsKey(Level.WARN));
            Assert.False(Called.ContainsKey(Level.INFO));
            Assert.False(Called.ContainsKey(Level.DEBUG));
            Assert.False(Called.ContainsKey(Level.TRACE));

            this.Logger.LoggingLevel = Level.WARN;
            Logger.Error("log");
            Logger.Trace("log");
            Logger.Info("log");
            Logger.Debug("log");
            Logger.Warning("log");
            Assert.True(Called.ContainsKey(Level.ERROR));
            Assert.True(Called.ContainsKey(Level.WARN));
            Assert.False(Called.ContainsKey(Level.INFO));
            Assert.False(Called.ContainsKey(Level.DEBUG));
            Assert.False(Called.ContainsKey(Level.TRACE));

            this.Logger.LoggingLevel = Level.INFO;
            Logger.Error("log");
            Logger.Trace("log");
            Logger.Info("log");
            Logger.Debug("log");
            Logger.Warning("log");
            Assert.True(Called.ContainsKey(Level.ERROR));
            Assert.True(Called.ContainsKey(Level.WARN));
            Assert.True(Called.ContainsKey(Level.INFO));
            Assert.False(Called.ContainsKey(Level.DEBUG));
            Assert.False(Called.ContainsKey(Level.TRACE));

            this.Logger.LoggingLevel = Level.DEBUG;
            Logger.Error("log");
            Logger.Trace("log");
            Logger.Info("log");
            Logger.Debug("log");
            Logger.Warning("log");
            Assert.True(Called.ContainsKey(Level.ERROR));
            Assert.True(Called.ContainsKey(Level.WARN));
            Assert.True(Called.ContainsKey(Level.INFO));
            Assert.True(Called.ContainsKey(Level.DEBUG));
            Assert.False(Called.ContainsKey(Level.TRACE));

            this.Logger.LoggingLevel = Level.TRACE;
            Logger.Error("log");
            Logger.Trace("log");
            Logger.Info("log");
            Logger.Debug("log");
            Logger.Warning("log");
            Assert.True(Called.ContainsKey(Level.ERROR));
            Assert.True(Called.ContainsKey(Level.WARN));
            Assert.True(Called.ContainsKey(Level.INFO));
            Assert.True(Called.ContainsKey(Level.DEBUG));
            Assert.True(Called.ContainsKey(Level.TRACE));
        }
    }
}