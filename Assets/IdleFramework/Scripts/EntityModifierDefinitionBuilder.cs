using System.Collections.Generic;

namespace IdleFramework
{
    internal class EntityModifierDefinitionBuilder
    {
        private ISet<StateMatcher> triggerMatchers = new HashSet<StateMatcher>();
        /*
         * A modifier which affect Entities.
         */
        public EntityModifierDefinitionBuilder()
        { }

        /*
         * Configure the conditions for when this modifier is applied.
         */
        public EntityModifierDefinitionBuilder When()
        {
            return new EntityModifierDefinitionBuilder();
        }

        public class EntityModifierTriggerConfiguration
        {
            private EntityModifierDefinitionBuilder parent;

            private EntityModifierTriggerConfiguration(EntityModifierDefinitionBuilder parent)
            {
                this.parent = parent;
            }
        }

    }
}