using BreakInfinity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public class EntityProductionHook : EngineHookDefinition<GameEntity, BigDouble>
    {
        public EntityProductionHook(string producingEntity, string producedEntity, Func<GameEntity, BigDouble> hook) : base(new EngineHookSelector(EngineHookAction.WILL_PRODUCE, producingEntity, producedEntity), hook)
        {

        }

        new public class Builder
        {
            private string producer;
            private string produced;
            private Func<GameEntity, BigDouble> hook;
            public EntityProductionSubjectBuilder WhenAnyEntity()
            {
                producer = "*";
                return new EntityProductionSubjectBuilder(this);
            }

            public EntityProductionSubjectBuilder WhenEntity(string entityKey)
            {
                producer = entityKey;
                return new EntityProductionSubjectBuilder(this);
            }

            public class EntityProductionSubjectBuilder
            {
                private Builder parent;

                public EntityProductionSubjectBuilder(Builder parent)
                {
                    this.parent = parent;
                }

                public EntityProductionHookAction ProducesAnyEntity()
                {
                    parent.produced = "*";
                    return new EntityProductionHookAction(parent);
                }

                public EntityProductionHookAction Produces(string entity)
                {
                    parent.produced = entity;
                    return new EntityProductionHookAction(parent);
                }
            }

            public class EntityProductionHookAction
            {
                Builder parent;
                private Func<GameEntity, BigDouble> hook;

                public EntityProductionHookAction(Builder parent)
                {
                    this.parent = parent;
                }

                public EntityProductionHookTerminal ThenExecute(Func<GameEntity, BigDouble> hook)
                {
                    this.hook = hook;
                    return new EntityProductionHookTerminal(parent);
                }
            }

            public class EntityProductionHookTerminal
            {
                private Builder parent;

                public EntityProductionHookTerminal(Builder parent)
                {
                    this.parent = parent;
                }

                public EntityProductionHook Build()
                {
                    return new EntityProductionHook(parent.producer, parent.produced, parent.hook);
                }
            }
        }
    }
}