using BreakInfinity;
using System;
using System.Collections.Generic;

namespace IdleFramework
{
    public class NothingEntityEffect : EntityEffectDefinition
    {
        
        public override BigDouble CalculateEffect(ModifiableProperty target, IdleEngine engine)
        {
            return target.Value;
        }

        public override IReadOnlyList<ModifiableProperty> GetAffectableProperties(IdleEngine engine)
        {
            return new List<ModifiableProperty>().AsReadOnly();
        }
    }
}