using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IdleFramework;
using BreakInfinity;

namespace Tests
{
    public class HookManagerTest
    {
        private HookManager manager;
        private bool engineStartHookCalled;
        [SetUp]
        public void setup()
        {
            var startHooks = new List<EngineStartHook>();
            startHooks.Add(new EngineStartHook((object payload) =>
            {
                engineStartHookCalled = true;
                return null;
            }));
            var entityHooks = new List<EntityProductionHook>();
            entityHooks.Add(new EntityProductionHook("*", "*", (GameEntity entity) => {
                return entity.QuantityChangePerSecond.GetAsNumber(null) * 2;
            }));
            manager = new HookManager(startHooks, entityHooks);
            engineStartHookCalled = false;
        }
        [Test]
        public void ManagerCanExecuteEngineStartHook()
        {
            manager.ExecuteEngineStartHooks();
            Assert.IsTrue(engineStartHookCalled);
        }
    }
}