using BreakInfinity;
using IdleFramework;
using NUnit.Framework;

namespace Tests
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
            Assert.AreEqual(BigDouble.Floor(1), ed.BaseProductionOutputs["food"].GetAsNumber(null));
        }

        [Test]
        public void EntityDefinitionBuilderCanAddToProductionInputs()
        {
            EntityDefinition ed = new EntityDefinitionBuilder("foo")
                .WithConsumption("food", 1)
                .Build();
            Assert.AreEqual(BigDouble.Floor(1), ed.BaseProductionInputs["food"].GetAsNumber(null));
        }

        [Test]
        public void EntityDefinitionBuilderCanAddToUpkeed()
        {
            EntityDefinition ed = new EntityDefinitionBuilder("foo")
                .WithUpkeepRequirement("food", 1)
                .Build();
            Assert.AreEqual(BigDouble.Floor(1), ed.BaseUpkeep["food"].GetAsNumber(null));
        }

        [Test]
        public void EntityDefinitionBuilderSpecifyingProductionMultipleTimesOverwrites()
        {
            EntityDefinition ed = new EntityDefinitionBuilder("foo")
                .WithProduction("food", 1)
                .WithProduction("food", 2)
                .Build();
            Assert.AreEqual(BigDouble.Floor(2), ed.BaseProductionOutputs["food"].GetAsNumber(null));
        }

        [Test]
        public void EntityDefinitionBuilderSpecifyingConsumptionMultipleTimesOverwrites()
        {
            EntityDefinition ed = new EntityDefinitionBuilder("foo")
                .WithConsumption("food", 1)
                .WithConsumption("food", 2)
                .Build();
            Assert.AreEqual(BigDouble.Floor(2), ed.BaseProductionInputs["food"].GetAsNumber(null));
        }

        [Test]
        public void EntityDefinitionBuilderCanSpecifyAFlatMinimumProduction()
        {
            EntityDefinition ed = new EntityDefinitionBuilder("foo")
                .WithFlatMinimumProduction("food", 1)
                .Build();
            Assert.AreEqual(BigDouble.Floor(1), ed.BaseMinimumProductionOutputs["food"].GetAsNumber(null));
        }
    }
}