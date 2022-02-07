using BreakInfinity;
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

        [Test]
        public void CanDeclareAResourceDefinition()
        {
            EngineConfiguration configuration = new EngineConfiguration();
            configuration.DeclareResource("food");
            Assert.IsNotNull(configuration.Resources["food"]);
        }

        [Test]
        public void CanDeclareAResourceWithABaseIncome()
        {
            EngineConfiguration configuration = new EngineConfiguration();
            configuration.DeclareResource(new ResourceDefinitionBuilder("food").WithBaseIncome(1));
            Assert.AreEqual(BigDouble.One, configuration.Resources["food"].BaseIncome);
        }
    }
}