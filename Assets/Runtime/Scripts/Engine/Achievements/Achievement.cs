using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Engine.Achievements
{
    public class Achievement : IUpdateable
    {
        public bool Completed;
        public long Id { get; }
        public string Description { get; }
        public string CompletionExpression { get; }
        public string CompletionEffect { get; }
        public Achievement(long id, string description, string completionExpression, string completionEffect = "")
        {
            this.Id = id;
            Completed = false;
            this.Description = description;
            this.CompletionExpression = completionExpression;
            this.CompletionEffect = completionEffect;
        }

        public void Update(IdleEngine engine, float deltaTime)
        {
            throw new System.NotImplementedException();
        }
    }
}