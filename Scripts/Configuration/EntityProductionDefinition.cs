using BreakInfinity;
using System.Collections.Generic;

namespace IdleFramework
{
    /*
     * Defined how an entity produces.
     */
    public class EntityProductionDefinition
    {
        private string entityKey;
        private BigDouble quantity;
        private Dictionary<string, BigDouble> consumption;
        private Dictionary<string, BigDouble> requirements;
        private bool scaleProductionOnRequirements = false;

        /*
         * The unique key of the entity that is produced
         */
        public string EntityKey { get => entityKey; }
        /**
         * The quantity of then entity to produce.
         */
        public BigDouble Quantity { get => quantity; }
        /**
         * The entities and quantities consumed when production occurs.
         */
        public Dictionary<string, BigDouble> Consumption { get => requirements; }
        /*
         * If the production is scaled based on the percentage of consumed entities available. On true, if only a percentage of the consumption requirements are available, only that percentage are consumed and the same ratio of the produced quantity is.
         */
        public bool ScaleProductionOnRequirements { get => scaleProductionOnRequirements; }
        public Dictionary<string, BigDouble> Requirements { get => requirements; }
    }
}