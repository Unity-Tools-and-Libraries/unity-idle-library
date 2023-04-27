using System;
using System.Collections.Generic;
using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker;
using Newtonsoft.Json;

namespace io.github.thisisnozaku.idle.framework.Engine
{
    /**
     * Type representing a player in the game.
     */
    public class Player : Entity, IHasResources
    {
        public Dictionary<string, ResourceHolder> Resources = new Dictionary<string, ResourceHolder>();
        public Player(IdleEngine engine, long id) : base(engine, id)
        {
            
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