using BreakInfinity;
using IdleFramework;
using NUnit.Framework;

namespace Test
{
    public class EntityDefinitionBuilderTest
    {
        [Test]
        public void EntityDefinitionBuilderCanSpecifyTheEntityKey()
        {
            EntityDefinition ed = new EntityDefinitionBuilder("foo").Build();
            Assert.AreEqual("foo", ed.EntityKey);
        }

        [Test]
        public void EntityDefinitionBuilderCanAddToProductionOutputs()
        {
            EntityDefinition ed = new EntityDefinitionBuilder("foo")
                .WithProduction("food", 1)
                .Build();
            Assert.AreEqual(BigDouble.Floor(1), ed.BaseProductionOutputs["food"].Get(null));
        }

        [Test]
        public void EntityDefinitionBuilderCanAddToProductionInputs()
        {
            EntityDefinition ed = new EntityDefinitionBuilder("foo")
                .WithConsumption("food", 1)
                .Build();
            Assert.AreEqual(BigDouble.Floor(1), ed.BaseProductionInputs["food"].Get(null));
        }

        [Test]
        public void EntityDefinitionBuilderCanAddToUpkeed()
        {
            EntityDefinition ed = new EntityDefinitionBuilder("foo")
                .WithUpkeepRequirement("food", 1)
                .Build();
            Assert.AreEqual(BigDouble.Floor(1), ed.BaseUpkeep["food"].Get(null));
        }

        [Test]
        public void EntityDefinitionBuilderSpecifyingProductionMultipleTimesOverwrites()
        {
            EntityDefinition ed = new EntityDefinitionBuilder("foo")
                .WithProduction("food", 1)
                .WithProduction("food", 2)
                .Build();
            Assert.AreEqual(BigDouble.Floor(2), ed.BaseProductionOutputs["food"].Get(null));
        }

        [Test]
        public void EntityDefinitionBuilderSpecifyingConsumptionMultipleTimesOverwrites()
        {
            EntityDefinition ed = new EntityDefinitionBuilder("foo")
                .WithConsumption("food", 1)
                .WithConsumption("food", 2)
                .Build();
            Assert.AreEqual(BigDouble.Floor(2), ed.BaseProductionInputs["food"].Get(null));
        }
    }
}