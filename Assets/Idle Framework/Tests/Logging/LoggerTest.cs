using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace io.github.thisisnozaku.idle.framework.Tests
{
    public class LoggerTest : RequiresEngineTests
    {
        
        [SetUp]
        public void setup()
        {
            
        }
        
        [Test]
        public void LogsAtErrorLevelByDefault()
        {
            LogAssert.Expect(UnityEngine.LogType.Error, "[] message");
            
            engine.Log(UnityEngine.LogType.Exception, "message");
            engine.Log(LogType.Log, "message");

            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void LogsAtSpecifiedLevelAndAbove()
        {
            engine.ConfigureLogging("*", LogType.Log);

            LogAssert.Expect(LogType.Log, "[] message");
            LogAssert.Expect(LogType.Error, "[] message");
            LogAssert.Expect(LogType.Warning, "[] message");
            

            engine.Log(LogType.Log, "message");
            engine.Log(LogType.Error, "message");
            engine.Log(LogType.Warning, "message");

            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void CanCustomizeLevelByContext()
        {
            engine.ConfigureLogging("combat", LogType.Log);
            LogAssert.Expect(LogType.Error, "[] message");
            LogAssert.Expect(LogType.Log, "[combat] message");

            engine.Log(LogType.Exception, "message");
            engine.Log(LogType.Log, "message", "combat");

            LogAssert.NoUnexpectedReceived();
        }
    }
}