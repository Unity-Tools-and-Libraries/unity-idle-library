using BreakInfinity;
using System.Collections.Generic;
using System.Numerics;

namespace IdleFramework
{
    /**
     * A Modifier is something which modified the properties of an entity.
     */ 
    public class Modifier
    {
        private readonly Dictionary<string, BigDouble> flatProductionModifiers = new Dictionary<string, BigDouble>();
        private bool isActive = false;
        public delegate bool Activator(IdleEngine engine);
        private Activator activator = (e) => true;

        public Dictionary<string, BigDouble> FlatProductionModifiers => flatProductionModifiers;
        public bool IsActive(IdleEngine engine) => activator(engine);

        public Modifier WithFlatProductionModifier(string entity, BigDouble modifier)
        {
            flatProductionModifiers[entity] = modifier;
            return this;
        }

        public Modifier WithActivator(Activator activator)
        {
            this.activator = activator;
            return this;
        }
    }
}