using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Modifiers;
using io.github.thisisnozaku.idle.framework.Modifiers.Values;
using System.Linq;
namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Clicker.Definitions.Upgrades
{
    public class MultiplierProducerUpgradeDefinition : UpgradeDefinition
    {
        public BigDouble Value { get; }
        public MultiplierProducerUpgradeDefinition(string Id, string Name, BigDouble cost, BigDouble value, params ProducerDefinition[] upgradeTargets) : base(Id, Name, cost, upgradeTargets.Select(x => x.Id).ToArray())
        {
            this.Value = value;
        }

        public override ContainerModifier GenerateModifier()
        {
            return new MultiplicativeValueModifier(Id, Id, Value);
        }
    }
}