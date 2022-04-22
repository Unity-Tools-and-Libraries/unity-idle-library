using BreakInfinity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

namespace io.github.thisisnozaku.idle.framework
{
    /*
     * A Dictionary where updates in contained values
     */
     [ExcludeFromCoverage]
    public class ParentNotifyingDictionary : IDictionary<string, ValueContainer>
    {
        private IdleEngine engine;
        private IDictionary<string, ValueContainer> underlying = new Dictionary<string, ValueContainer>();
        private List<Action<object>> listeners = new List<Action<object>>();

        public ParentNotifyingDictionary(IdleEngine engine): this(engine, null)
        {
            
        }

        public ParentNotifyingDictionary(IdleEngine engine, IDictionary<string, ValueContainer> other)
        {
            if (other != null)
            {
                underlying = new Dictionary<string, ValueContainer>(other);
            } else
            {
                underlying = new Dictionary<string, ValueContainer>();
            }
            this.engine = engine;
        }
        public ValueContainer this[string key]
        {
            get
            {
                ValueContainer existing;
                underlying.TryGetValue(key, out existing);
                
                return existing;
            }
            set {
                underlying[key] = value;
                value.Watch(x => NotifyListeners());
            }
        }

        public ICollection<string> Keys => underlying.Keys;

        public ICollection<ValueContainer> Values => underlying.Values;

        public int Count => underlying.Count;

        public bool IsReadOnly => false;

        public void Add(string key, ValueContainer value)
        {
            underlying[key] = value;
        }

        public void Add(KeyValuePair<string, ValueContainer> item)
        {
            underlying[item.Key] = item.Value;
        }

        public void Clear()
        {
            foreach(var key in underlying.Keys)
            {
                underlying[key].Set(ValueContainer.DEFAULT_VALUE);
            }
        }

        public bool ContainsKey(string key)
        {
            return underlying.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, ValueContainer>[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator<KeyValuePair<string, ValueContainer>> GetEnumerator()
        {
            return underlying.GetEnumerator();
        }

        public bool Remove(string key)
        {
            if(underlying.ContainsKey(key))
            {
                underlying[key].Set((string)null);
                return true;
            }
            return false;
        }

        public bool Remove(KeyValuePair<string, ValueContainer> item)
        {
            throw new System.NotImplementedException();
        }

        public bool TryGetValue(string key, out ValueContainer value)
        {
            throw new System.NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return underlying.GetEnumerator();
        }

        public void NotifyListeners()
        {
            foreach(var listener in listeners)
            {
                listener.Invoke(this);
            }
        }

        public bool Contains(KeyValuePair<string, ValueContainer> item)
        {
            return underlying.Contains(item);
        }
    }
}