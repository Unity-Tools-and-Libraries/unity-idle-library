using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Modifiers;
using System.Collections.Generic;

namespace io.github.thisisnozaku.idle.framework.Configuration
{
    public class ResourceDefinition : ValueContainerDefinition
    {
        private string Id;
        public readonly BigDouble BaseIncome;


        internal ResourceDefinition(string name, BigDouble baseIncome) : base(generateStartingParameter(baseIncome), null, null, new List<ValueModifier>())
        {
            this.Id = name;
            this.BaseIncome = baseIncome;
        }

        private static IDictionary<string, ValueContainerDefinition> generateStartingParameter(BigDouble baseIncome)
        {
            return new Dictionary<string, ValueContainerDefinition>()
            {
                { "quantity", new ValueContainerDefinitionBuilder().WithUpdater((e, dt, pv, parent, mods)=> {
                    return ((BigDouble)pv).Add(parent.ValueAsMap()["income"].ValueAsNumber().Multiply(dt));
                }).Build()},
                { "income", new ValueContainerDefinitionBuilder().WithUpdater((e, dt, pv, parent, mods) => {
                    var additiveMods = parent.GetModifiers();
                    return BigDouble.One;
                }).WithModifier(new AdditiveValueModifier("baseIncome", "Base", baseIncome)).Build() }
            };
        }
    }
}