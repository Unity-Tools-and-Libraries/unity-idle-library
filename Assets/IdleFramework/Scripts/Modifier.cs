using System;
using System.Collections.Generic;

namespace IdleFramework
{
    public class Modifier: ModifierDefinitionProperties
    {
        private readonly ModifierDefinition definition;
        private readonly IdleEngine engine;
        private readonly ISet<Effect> effects;
        public Modifier(ModifierDefinition definition, ISet<Effect> effects)
        {
            this.definition = definition;
            this.effects = effects;
        }

        public Modifier(ModifierDefinition definition): this(definition, new HashSet<Effect>())
        {

        }

        public ModifierDefinition Defintion => definition;

        public string ModifierKey => definition.ModifierKey;

        public IEnumerable<Effect> Effects => effects;

        internal bool IsActive(IdleEngine idleEngine)
        {
            return definition.Trigger.Matches(idleEngine);
        }
    }
}