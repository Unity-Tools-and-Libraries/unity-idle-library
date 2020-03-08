using System;
using System.Numerics;

namespace IdleFramework
{
    /*
     * Contains the state of a Resource.
     */
    public class Resource : GameEntity, Updates
    {
        public Resource(EntityDefinition definition, IdleEngine engine) : base(definition, engine)
        {

        }
    }
}