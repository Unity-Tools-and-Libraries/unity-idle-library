using BreakInfinity;
using System;

namespace IdleFramework
{
    public class NothingEntityEffect : EntityEffectDefinition
    {
        
        public override BigDouble CalculateEffect(ModifiableProperty target, IdleEngine engine)
        {
            return target.Value;
        }
    }
}