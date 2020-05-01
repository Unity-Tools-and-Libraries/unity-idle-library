using BreakInfinity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{

    /*
     * Interface for a stateless entity definition.
     */
    public interface IEntityDefinition
    {
        BigDouble StartingQuantity { get; }
        string EntityKey { get; }
        string VariantKey { get; }
        StringContainer Name { get; }
        Dictionary<string, NumberContainer> BaseScaledInputs { get; }
        Dictionary<string, NumberContainer> BaseRequirements { get; }
        Dictionary<string, NumberContainer> BaseCosts { get; }
        Dictionary<string, NumberContainer> BaseScaledOutputs { get; }
        Dictionary<string, NumberContainer> BaseFixedInputs { get; }
        Dictionary<string, NumberContainer> BaseFixedOutputs { get; }
        Dictionary<string, NumberContainer> BaseUpkeep { get; }
        ISet<string> Types { get; }
        Dictionary<string, EntityDefinition> Variants { get; }
        StateMatcher IsVisibleMatcher { get; }
        StateMatcher IsAvailableMatcher { get; }
        StateMatcher IsEnabledMatcher { get; }
        NumberContainer QuantityCap { get; }
        NumberContainer CalculatedQuantity { get; }
        PropertyHolder CustomProperties { get; }
        bool Accumulates { get; }
        IList<ModifierDefinition> Modifiers { get; }
    }
}