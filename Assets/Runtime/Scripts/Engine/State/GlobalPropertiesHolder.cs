using System;
using System.Collections;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;

public class PropertiesHolder : IDictionary<string, object>, IUserDataType
{
    private Dictionary<string, object> properties = new Dictionary<string, object>();
    public object this[string id]
    {
        get { return properties.GetValueOrDefault<string, object>(id, null); }
        set { properties[id] = value; }
    }

    public ICollection<string> Keys => ((IDictionary<string, object>)properties).Keys;

    public ICollection<object> Values => ((IDictionary<string, object>)properties).Values;

    public int Count => ((ICollection<KeyValuePair<string, object>>)properties).Count;

    public bool IsReadOnly => ((ICollection<KeyValuePair<string, object>>)properties).IsReadOnly;

    public void Add(string key, object value)
    {
        ((IDictionary<string, object>)properties).Add(key, value);
    }

    public void Add(KeyValuePair<string, object> item)
    {
        ((ICollection<KeyValuePair<string, object>>)properties).Add(item);
    }

    public void Clear()
    {
        ((ICollection<KeyValuePair<string, object>>)properties).Clear();
    }

    public bool Contains(KeyValuePair<string, object> item)
    {
        return ((ICollection<KeyValuePair<string, object>>)properties).Contains(item);
    }

    public bool ContainsKey(string key)
    {
        return ((IDictionary<string, object>)properties).ContainsKey(key);
    }

    public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
    {
        ((ICollection<KeyValuePair<string, object>>)properties).CopyTo(array, arrayIndex);
    }

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
    {
        return ((IEnumerable<KeyValuePair<string, object>>)properties).GetEnumerator();
    }

    public DynValue Index(Script script, DynValue index, bool isDirectIndexing)
    {
        return DynValue.FromObject(script, properties.GetValueOrDefault<string, object>(index.CastToString(), null));
    }

    public DynValue MetaIndex(Script script, string metaname)
    {
        throw new System.NotImplementedException();
    }

    public bool Remove(string key)
    {
        return ((IDictionary<string, object>)properties).Remove(key);
    }

    public bool Remove(KeyValuePair<string, object> item)
    {
        return ((ICollection<KeyValuePair<string, object>>)properties).Remove(item);
    }

    public bool SetIndex(Script script, DynValue index, DynValue value, bool isDirectIndexing)
    {
        properties[index.CastToString()] = value.ToObject();
        return true;
    }

    public bool TryGetValue(string key, out object value)
    {
        return ((IDictionary<string, object>)properties).TryGetValue(key, out value);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)properties).GetEnumerator();
    }
}
