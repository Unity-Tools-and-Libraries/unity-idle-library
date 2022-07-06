using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Modifiers;
using NUnit.Framework;

namespace io.github.thisisnozaku.idle.framework.Tests.Modifiers.BasicOperations
{
    public class DivisionModifierTests: RequiresEngineTests
    {
        [Test]
        public void DivisionModifierCanDivideBySetValue()
        {
            engine.Start();
            var vc = engine.CreateProperty("path", 0, "", new System.Collections.Generic.List<IContainerModifier>()
            {
                new ValueModifier("", "", "return value / 2", new ContainerScriptingContextHolder(engine, "path"))
            });
            vc.Set(10);
            Assert.AreEqual(new BigDouble(5), vc.ValueAsNumber());
        }

        [Test]
        public void DivisionModifierCanHaveDynamicValue()
        {
            engine.Start();
            var vc = engine.CreateProperty("path", 0, "", new System.Collections.Generic.List<IContainerModifier>()
            {
                new ValueModifier("", "", "return value / foo.bar", engine, "return foo != null")
            });
            engine.GetProperty("foo.bar", framework.Engine.IdleEngine.GetOperationType.GET_OR_CREATE).Set(2);
            vc.Set(10);
            Assert.AreEqual(new BigDouble(5), vc.ValueAsNumber());
            engine.GetProperty("foo.bar").Set(5);
            engine.Update(0);
            Assert.AreEqual(new BigDouble(2), vc.ValueAsNumber());
        }
    }
}