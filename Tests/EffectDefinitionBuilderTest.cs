using IdleFramework;
using NUnit.Framework;

namespace Tests
{
    public class ModifierDefinitionBuilderTest
    {
        [Test]
        public void ModifierDefinitionBuilderCanCreateAnEffectWithAKey()
        {
            ModifierDefinition definition = new ModifierDefinitionBuilder("test-effect").Active().Always().And().HasEntityEffect(null).Build();

            Assert.AreEqual(definition.ModifierKey, "test-effect");
        }

        [Test]
        public void ModifierDefinitionBuilderCanAddAConstantStateMatcher()
        {
            ModifierDefinition definition = new ModifierDefinitionBuilder("test-effect").Active().Always().And().HasEntityEffect(null).Build();

            Assert.AreEqual(definition.ModifierKey, "test-effect");
        }

        [Test]
        public void ModifierDefinitionBuilderThrowsErrorWhenNoMatchers()
        {
            Assert.That(() => new ModifierDefinitionBuilder("test-effect").Build(), Throws.InvalidOperationException);
        }

        [Test]
        public void ModifierDefinitionBuilderThrowsErrorWhenNoEffects()
        {
            Assert.That(() => new ModifierDefinitionBuilder("test-effect").Active().Always().Build(), Throws.InvalidOperationException);
        }
    }
}
