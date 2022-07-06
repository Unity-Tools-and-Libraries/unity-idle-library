using io.github.thisisnozaku.idle.framework.Engine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Modifiers
{
    public class ValueModifier : ContainerModifier
    {
        private string Id;
        private string expression;
        private string checkExpression;
        private string description;
        public ValueModifier(string id, string description, string operationExpression, ScriptingContext context, string canApplyCheckExpression = "", int priority = 0) : base(id, description, context, priority)
        {
            if(!operationExpression.StartsWith("return"))
            {
                throw new ArgumentException("Expression must start with return so that it returns a value.");
            }
            this.Id = id;
            this.expression = operationExpression;
            this.checkExpression = canApplyCheckExpression;
            this.description = description;
        }

        public override object Apply(IdleEngine engine, ValueContainer container, object input)
        {
            object result = engine.EvaluateExpression(expression, new Dictionary<string, object>()
            {
                { "value", input },
                { "target", container }
            });
            if(result is ValueContainer)
            {
                var containerResult = result as ValueContainer;
                switch (container.DataType)
                {
                    case "string":
                        return containerResult.AsString;
                    case "number":
                        return containerResult.AsNumber;
                    case "bool":
                        return container.AsBool;
                    case "list":
                        return container.AsList;
                    case "map":
                        return container.AsMap;
                }
                
            }
            return result;
        }

        public override bool CanApply(IdleEngine engine, ValueContainer container, object intermediateValue)
        {
            if (checkExpression == "")
            {
                return true;
            }
            bool canApply = (bool)engine.EvaluateExpression(checkExpression, new Dictionary<string, object>(){
                { "value", intermediateValue }
            });
            return canApply;
        }

        public override string ToString()
        {
            return String.Format("{0} {1} ({2})", GetType().Name, Id, expression);
        }

        public static class DefaultPriorities
        {
            public const int ADDITION = 1000;
            public const int MULTIPLICATION = 2000;
        }
    }
}