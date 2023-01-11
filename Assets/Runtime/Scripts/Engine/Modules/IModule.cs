
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules
{
    /**
     * Interface for a class which encapsulates a set of related configuration for the engine.
     * 
     * Most methods are related to performing different stages of configuration and are called in the following order:
     * - SetConfiguration
     * - SetDefinitions
     */
    public interface IModule
    {
        /*
         * This method is intended to be used to load
         */
        void SetConfiguration(IdleEngine engine);
        void AssertReady();
        void LoadScripts(IdleEngine engine);
    }
}