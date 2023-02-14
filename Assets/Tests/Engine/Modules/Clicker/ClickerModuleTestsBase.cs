using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Definitions;
using NUnit.Framework;
using System;

namespace io.github.thisisnozaku.idle.framework.Tests.Engine.Modules.Clicker
{
    public class ClickerModuleTestsBase : TestsRequiringEngine
    {
        protected ClickerModule module;

        [SetUp]
        public void setup()
        {
            module = new ClickerModule();

            module.AddProducer(new Producer(engine, 1, "one", Tuple.Create<string, string>("points", "return 1 * math.pow(1.15, level)"), 1));

            module.AddUpgrade(new Upgrade(engine, 2, "", Tuple.Create<string, string>("points", "return 100 * math.pow(1.15, level)"), "return true", "return true", new System.Collections.Generic.Dictionary<string, System.Tuple<string, string>>()
            {
                { "producers[1].output_multiplier", Tuple.Create<string, string>("value * 2", null) }
            }, new BigDouble(100)));
            module.AddUpgrade(new Upgrade(engine, 3, "", Tuple.Create<string, string>("points", "return level * 10"), "return true", "return true", new System.Collections.Generic.Dictionary<string, Tuple<string, string>>(), 100));
        }

        public void Configure()
        {
            engine.AddModule(module);
        }
    }
}