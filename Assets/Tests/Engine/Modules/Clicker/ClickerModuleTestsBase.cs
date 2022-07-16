using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Definitions;
using NUnit.Framework;

namespace io.github.thisisnozaku.idle.framework.Tests.Engine.Modules.Clicker
{
    public class ClickerModuleTestsBase : TestsRequiringEngine
    {
        protected ClickerModule module;

        [SetUp]
        public void setup()
        {
            base.InitializeEngine();
            module = new ClickerModule();

            module.AddProducer(new Producer(engine, 1, "one", 1, 1));

            module.AddUpgrade(new Upgrade(engine, 2, "", 1, "return true", "return true", new System.Collections.Generic.Dictionary<string, System.Tuple<string, string>>()));
        }

        public void Configure()
        {
            engine.AddModule(module);
        }
    }
}