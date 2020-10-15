using BreakInfinity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework
{
    public class ParentNotifyingMap : IDictionary<string, ValueReference>, Watchable
    {
        private Dictionary<string, ValueReference> underlying = new Dictionary<string, ValueReference>();
        private List<Action<object>> listeners = new List<Action<object>>();

        internal ParentNotifyingMap(): this(null)
        {
            
        }

        internal ParentNotifyingMap(IDictionary<string, ValueReference> other)
        {
            if (other != null)
            {
                underlying = new Dictionary<string, ValueReference>(other);
            } else
            {
                underlying = new Dictionary<string, ValueReference>();
            }
        }
        public ValueReference this[string key]
        {
            get
            {
                ValueReference existing;
                if(!underlying.TryGetValue(key, out existing))
                {
                    existing = new ValueReference();
                    underlying[key] = existing;
                    existing.Watch(x => NotifyListeners());
                }
                return existing;
            }
            set {
                underlying[key] = value;
                value.Watch(x => NotifyListeners());
            }
        }

        public ICollection<string> Keys => underlying.Keys;

        public ICollection<ValueReference> Values => underlying.Values;

        public int Count => underlying.Count;

        public bool IsReadOnly => throw new System.NotImplementedException();

        public void Add(string key, ValueReference value)
        {
            throw new System.NotImplementedException();
        }

        public void Add(KeyValuePair<string, ValueReference> item)
        {
            throw new System.NotImplementedException();
        }

        public void Clear()
        {
            throw new System.NotImplementedException();
        }

        public bool Contains(KeyValuePair<string, ValueReference> item)
        {
            throw new System.NotImplementedException();
        }

        public bool ContainsKey(string key)
        {
            throw new System.NotImplementedException();
        }

        public void CopyTo(KeyValuePair<string, ValueReference>[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator<KeyValuePair<string, ValueReference>> GetEnumerator()
        {
            return underlying.GetEnumerator();
        }

        public bool Remove(string key)
        {
            throw new System.NotImplementedException();
        }

        public bool Remove(KeyValuePair<string, ValueReference> item)
        {
            throw new System.NotImplementedException();
        }

        public bool TryGetValue(string key, out ValueReference value)
        {
            throw new System.NotImplementedException();
        }

        public void Watch(Action<object> listener)
        {
            this.listeners.Add(listener);
            listener.Invoke(this);
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
    }
}