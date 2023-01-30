using System.Collections;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using Newtonsoft.Json;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Engine.Achievements
{
    public class Achievement : IUpdateable
    {
        public bool Completed;
        public long Id { get; }
        public string Description { get; }
        [JsonIgnore]
        public DynValue CompletionExpression { get; }
        [JsonIgnore]
        public DynValue CompletionEffect { get; }

        [JsonConstructor]
        public Achievement(long id, string description, string completionExpression, string completionEffect = "") : this(id, description, DynValue.FromObject(null, completionExpression), DynValue.FromObject(null, completionEffect))
        {
            
        }

        public Achievement(long id, string description, DynValue completionTrigger, DynValue completionEffect = null)
        {
            this.Id = id;
            Completed = false;
            this.Description = description;
            this.CompletionExpression = completionTrigger;
            this.CompletionEffect = completionEffect;
        }

        public void Update(IdleEngine engine, float deltaTime)
        {
            throw new System.NotImplementedException();
        }
    }
}