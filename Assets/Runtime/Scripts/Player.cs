using System;
using System.Collections.Generic;
using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker;

namespace io.github.thisisnozaku.idle.framework.Engine
{
    /**
     * Type representing a player in the game.
     */
    public class Player : Entity, IHasResources
    {
        private Dictionary<string, ResourceHolder> Resources = new Dictionary<string, ResourceHolder>();
        public Player(IdleEngine engine, long id, Dictionary<string, BigDouble> resources) : base(engine, id)
        {
            if (resources != null) // So no error in deserialization
            {
                foreach (var e in resources)
                {
                    Resources[e.Key] = new ResourceHolder();
                    Resources[e.Key].Quantity = e.Value;
                }
            }
        }

        public ResourceHolder GetResource(string id)
        {
            ResourceHolder resource;
            if(!Resources.TryGetValue(id, out resource))
            {
                throw new InvalidOperationException("No resource with id " + id);
            }
            return resource;
        }
    }
}