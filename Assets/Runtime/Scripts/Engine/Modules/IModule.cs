
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules
{
    public interface IModule
    {
        /*
         * Use this method to set configuration values. Called as the first configuration method.
         */
        void SetConfiguration(IdleEngine engine);
        /*
         * Use this method to register definitions with the engine. Called after SetConfiguration.
         */
        void SetDefinitions(IdleEngine engine);
        /*
         * Use this method to set global properties on the engine. Called after SetDefinitions.
         */
        void SetGlobalProperties(IdleEngine engine);
        void AssertReady();
    }
}