using BreakInfinity;

namespace IdleFramework
{
   /*
    * An effect which modifies an entity property.
    */
    public struct EntityPropertyEffect
    {
        public readonly string property;
        public readonly EffectType type;
        public readonly BigDouble value;
    }
}