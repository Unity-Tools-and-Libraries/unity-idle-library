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
                .WithScaledOutput("food", 1)
                .Build();
            Assert.AreEqual(BigDouble.Floor(1), ed.BaseScaledOutputs["food"].Get(null));
        }

        [Test]
        public void EntityDefinitionBuilderCanAddToProductionInputs()
        {
            EntityDefinition ed = new EntityDefinitionBuilder("foo")
                .WithScaledInput("food", 1)
                .Build();
            Assert.AreEqual(BigDouble.Floor(1), ed.BaseScaledInputs["food"].Get(null));
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
                .WithScaledOutput("food", 1)
                .WithScaledOutput("food", 2)
                .Build();
            Assert.AreEqual(BigDouble.Floor(2), ed.BaseScaledOutputs["food"].Get(null));
        }

        [Test]
        public void EntityDefinitionBuilderSpecifyingConsumptionMultipleTimesOverwrites()
        {
            EntityDefinition ed = new EntityDefinitionBuilder("foo")
                .WithScaledInput("food", 1)
                .WithScaledInput("food", 2)
                .Build();
            Assert.AreEqual(BigDouble.Floor(2), ed.BaseScaledInputs["food"].Get(null));
        }
    }
}