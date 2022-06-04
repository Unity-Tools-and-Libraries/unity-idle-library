using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Modifiers.Values;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Modifiers
{
    /*
     * Clamps the value to be between the floor and ceiling values.
     * 
     * If the input value is above the ceiling value, the ceiling is returned; if the input value is below the floor value, the floor is returned.
     */
    public class ClampValueModifier : ValueModifier
    {
        private string floorExpression;
        private string ceilingExpression;
        private BigDouble floorValue;
        private BigDouble ceilingValue;
        private bool cacheFloor;
        private bool cacheCeiling;

        public ClampValueModifier(string id, string source, string floorValueExpression, string ceilingValueExpression) : base(id, source, null, false, 0)
        {
            this.floorExpression = floorValueExpression;
            this.ceilingExpression = ceilingValueExpression;
        }

        public ClampValueModifier(string id, string source, string floorValueExpression, BigDouble ceilingValue) : base(id, source, null, false, 0)
        {
            this.floorExpression = floorValueExpression;
            this.ceilingValue = ceilingValue;
            cacheCeiling = true;
        }

        public ClampValueModifier(string id, string source, BigDouble floorValue, string ceilingValueExpression) : base(id, source, null, false, 0)
        {
            this.floorValue = floorValue;
            this.ceilingExpression = ceilingValueExpression;
            cacheFloor = true;
        }

        public ClampValueModifier(string id, string source, BigDouble floorValue, BigDouble ceilingValue) : base(id, source, null, false, 0)
        {
            this.floorValue = floorValue;
            this.ceilingValue = ceilingValue;
            cacheFloor = true;
            cacheCeiling = true;
        }

        public BigDouble EvaluateCeiling(IdleEngine engine)
        {
            if (cacheCeiling && ceilingValue != null)
            {
                return ceilingValue;
            }
            var evaluatedExpression = engine.EvaluateExpression(ceilingExpression);
            return (BigDouble)ValueContainer.NormalizeValue(evaluatedExpression);
        }

        public BigDouble EvaluateFloor(IdleEngine engine)
        {
            if (cacheFloor && floorValue != null)
            {
                return floorValue;
            }
            var evaluatedExpression = engine.EvaluateExpression(floorExpression);
            return (BigDouble)ValueContainer.NormalizeValue(evaluatedExpression);
        }

        public override object Apply(IdleEngine engine, ValueContainer container, object input)
        {
            return BigDouble.Max(EvaluateFloor(engine),
                BigDouble.Min(EvaluateCeiling(engine), (BigDouble)input));
        }
    }
}