using io.github.thisisnozaku.idle.framework.Engine.Logging;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace io.github.thisisnozaku.idle.framework.Tests
{
    public class LoggerTest
    {
        LoggingService logger;
        
        [SetUp]
        public void setup()
        {
            logger = new LoggingService();
        }
        
        [Test]
        public void LogsAtErrorLevelByDefault()
        {
            LogAssert.Expect(UnityEngine.LogType.Error, "[*] message");
            
            logger.Log(UnityEngine.LogType.Error, "message");
            logger.Log(LogType.Log, "message");

            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void LogsAtSpecifiedLevelAndAbove()
        {
            logger.ConfigureLogging("*", LogType.Log);

            LogAssert.Expect(LogType.Log, "[*] message");
            LogAssert.Expect(LogType.Error, "[*] message");
            LogAssert.Expect(LogType.Warning, "[*] message");


            logger.Log(LogType.Log, "message");
            logger.Log(LogType.Error, "message");
            logger.Log(LogType.Warning, "message");

            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void CanCustomizeLevelByContext()
        {
            logger.ConfigureLogging("combat", LogType.Log);
            LogAssert.Expect(LogType.Error, "[*] message");
            LogAssert.Expect(LogType.Log, "[combat] message");

            logger.Log(LogType.Error, "message");
            logger.Log(LogType.Log, "message", "combat");

            LogAssert.NoUnexpectedReceived();
        }
    }
}