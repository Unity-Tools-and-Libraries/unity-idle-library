using IdleFramework.Configuration;
using NUnit.Framework;
using System;

namespace IdleFramework.Tests
{
    public class IdleEngineConfigurationTest
    {

        [Test]
        public void EngineConfigurationCanDeclareAGlobalBooleanProperty()
        {
            EngineConfiguration configuration = new EngineConfiguration();
            Assert.DoesNotThrow(() =>
            {
                configuration.DeclareGlobalProperty("booleanProperty");
            });
        }

        [Test]
        public void CanReadDeclaredGlobalBooleanPropertyFromEngineConfiguration()
        {
            EngineConfiguration configuration = new EngineConfiguration();
            configuration.DeclareGlobalProperty("booleanProperty");
            Assert.AreEqual(1, configuration.GlobalProperties.Count);
            Assert.NotNull(configuration.GlobalProperties["booleanProperty"]);
        }

        [Test]
        public void CannotModifyGlobalPropertiesDictionaryDirectly()
        {
            EngineConfiguration configuration = new EngineConfiguration();
            configuration.DeclareGlobalProperty("booleanProperty");
            Assert.Throws<NotSupportedException>(() => {
                configuration.GlobalProperties["booleanProperty"] = null;
            });
        }
    }
}