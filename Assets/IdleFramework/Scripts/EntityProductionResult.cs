using BreakInfinity;
using System.Collections.Generic;

namespace IdleFramework
{
    /*
     * Information for entity production which is going to occur to be passed into hooks for modification.
     */
    public struct EntityProductionResult : HookExecutionContext<EntityProductionResult>
    {
        private readonly GameEntity entity;
        private readonly Dictionary<string, BigDouble> inputsToConsume;
        private readonly Dictionary<string, BigDouble> outputsToProduce;

        public EntityProductionResult(GameEntity entity, Dictionary<string, BigDouble> inputsToConsume, Dictionary<string, BigDouble> outputsToProduce)
        {
            this.entity = entity;
            this.inputsToConsume = inputsToConsume;
            this.outputsToProduce = outputsToProduce;
        }

        public Dictionary<string, BigDouble> OutputsToProduce => outputsToProduce;

        public Dictionary<string, BigDouble> InputsToConsume => inputsToConsume;

        public GameEntity Entity => entity;

        public EntityProductionResult Payload => this;

        public string Actor => entity.EntityKey;
    }
}