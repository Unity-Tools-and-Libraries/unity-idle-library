using System.Collections;
using System.Collections.Generic;
using io.github.thisisnozaku.idle.framework.Engine;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.ui {
    /**
     * 
     * Component that holds an idle engine so that it can be exposed to the Unity 
     * component ecosystem.
     * 
     * Create a subclass of this where you perform your configuration and 
     * initialization of the engine and use fields of this class for assigning in 
     * the editor or doing lookups at runtime.
     */
    public class EngineHolder : MonoBehaviour
    {
        public IdleEngine engine;
    }
}
