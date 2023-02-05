using io.github.thisisnozaku.idle.framework;
using io.github.thisisnozaku.idle.framework.Engine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Engine
{
    /*
     * Abstract base class for things which "modify" entities, such as upgrades, special abilities, status effects, etc.
     */
    public abstract class EntityModifier<T> : Entity where T : Entity
    {
        private Dictionary<string, Tuple<string, string>> modifications;

        protected EntityModifier(IdleEngine engine, long id, Dictionary<string, Tuple<string, string>> modifications) : base(engine, id)
        {
            this.modifications = modifications;
        }

        public virtual void Apply(T target)
        {
            if (modifications != null)
            {
                foreach (var effect in modifications)
                {
                    var context = new Dictionary<string, object>() { { "target", target } };
                    string toExecute;
                    if (effect.Key.EndsWith("setFlag") || effect.Key.EndsWith("clearFlag"))
                    {
                        toExecute = string.Format("target.{1}('{0}')", effect.Value.Item1, effect.Key);
                    }
                    else
                    {
                        toExecute = string.Format("target.{0} = {1}", effect.Key, effect.Value.Item1);
                        context["value"] = Engine.Scripting.EvaluateStringAsScript(string.Format("return target.{0}", effect.Key), context).ToObject();
                    }

                    Engine.Scripting.EvaluateStringAsScript(toExecute, context);
                }
            }
        }
        public virtual void Unapply(T target)
        {
            if (modifications != null)
            {
                foreach (var effect in modifications)
                {
                    if (effect.Value.Item2 == null)
                    {
                        continue;
                    }
                    var context = new Dictionary<string, object>() { { "target", target } };
                    string toExecute;
                    switch (effect.Key)
                    {
                        case "setFlag":
                            toExecute = string.Format("target.ClearFlag('{0}')", effect.Value.Item2);
                            break;
                        case "clearFlag":
                            toExecute = string.Format("target.SetFlag('{0}')", effect.Value.Item2);
                            break;
                        default:
                            toExecute = string.Format("target.{0} = {1}", effect.Key, effect.Value.Item2);
                            context["value"] = Engine.Scripting.EvaluateStringAsScript(string.Format("return target.{0}", effect.Key), context).ToObject();
                            break;
                    }

                    Engine.Scripting.EvaluateStringAsScript(toExecute, context);
                }
            }
        }

        public abstract class Builder<B> where B : EntityModifier<T>
        {
            protected Dictionary<string, Tuple<string, string>> modifications = new Dictionary<string, Tuple<string, string>>();
            public abstract B Build(IdleEngine engine, long id);

            public Builder<B> ChangeProperty(string targetProperty, string changeScript, string undoScript = null)
            {
                modifications[targetProperty] = Tuple.Create(changeScript, undoScript);
                return this;
            }

            public Builder<B> SetFlag(string flag, bool unsetOnRemoval = false)
            {
                modifications["setFlag"] = Tuple.Create<string, string>(flag, unsetOnRemoval ? flag : null);
                return this;
            }

            public Builder<B> ClearFlag(string flag)
            {
                modifications["clearFlag"] = Tuple.Create<string, string>(flag, null);
                return this;
            }
        }
    }
}