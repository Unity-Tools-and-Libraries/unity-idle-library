using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IdleFramework;
using BreakInfinity;
using System;

namespace Tests
{
    public class HookManagerTest
    {
        private HookManager manager;
        private bool engineStartHookCalled;
        [SetUp]
        public void setup()
        {
            var startHooks = new List<Action<IdleEngine>>();
            startHooks.Add(engine =>
            {
                engineStartHookCalled = true;
            });
            var entityHooks = new List<EntityProductionHook>();
            entityHooks.Add(new EntityProductionHook("*", "*", (Entity entity) => {
                return entity.QuantityChangePerSecond * 2;
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