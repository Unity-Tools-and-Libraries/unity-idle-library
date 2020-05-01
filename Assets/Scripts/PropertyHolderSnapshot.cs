using BreakInfinity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropertyHolderSnapshot : Snapshot
{
    private readonly IEnumerable<KeyValuePair<string, BigDouble>> numberProperties;
    private readonly IEnumerable<KeyValuePair<string, string>> stringProperties;
    private readonly IEnumerable<KeyValuePair<string, bool>> booleanProperties;

    public PropertyHolderSnapshot(IEnumerable<KeyValuePair<string, BigDouble>> numberProperties, IEnumerable<KeyValuePair<string, string>> stringProperties, IEnumerable<KeyValuePair<string, bool>> booleanProperties)
    {
        this.numberProperties = numberProperties;
        this.stringProperties = stringProperties;
        this.booleanProperties = booleanProperties;
    }

    public IEnumerable<KeyValuePair<string, BigDouble>> NumberProperties => numberProperties;

    public IEnumerable<KeyValuePair<string, string>> StringProperties => stringProperties;

    public IEnumerable<KeyValuePair<string, bool>> BooleanProperties => booleanProperties;
}
