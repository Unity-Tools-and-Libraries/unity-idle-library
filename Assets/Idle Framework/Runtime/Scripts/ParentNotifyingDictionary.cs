using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Events;
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
        private ValueContainer parent;
        private Dictionary<string, ValueContainer> underlying = new Dictionary<string, ValueContainer>();
        private List<Action<object>> listeners = new List<Action<object>>();

        public ParentNotifyingDictionary(ValueContainer parent, IDictionary<string, ValueContainer> other = null)
        {
            if (parent == null)
            {
                throw new ArgumentNullException("parent");
            }
            this.parent = parent;
            if (other != null)
            {
                underlying = new Dictionary<string, ValueContainer>(other);
            }
            else
            {
                underlying = new Dictionary<string, ValueContainer>();
            }
            var children = underlying.GetEnumerator();
            while (children.MoveNext())
            {
                var child = children.Current;
                if(parent.Path != null)
                {
                    child.Value.Path = string.Join(".", parent.Path, child.Key);
                }
                child.Value.Parent = parent;
            }
        }
        public ValueContainer this[string key]
        {
            get
            {
                ValueContainer existing;
                underlying.TryGetValue(key, out existing);

                return existing;
            }
            set
            {
                ValueContainer previous;
                underlying.TryGetValue(key, out previous);
                underlying[key] = value;
                value.Parent = parent;
                if(parent.Path != null)
                {
                    value.Path = String.Join(".", parent.Path, key);
                }
                parent.NotifyImmediately(ChildValueChangedEvent.EventName, parent, value.Path, previous != null ? previous.ValueAsRaw() : null, value.ValueAsRaw());
            }
        }

        public ICollection<string> Keys => underlying.Keys;

        public ICollection<ValueContainer> Values => underlying.Values;

        public int Count => underlying.Count;

        public bool IsReadOnly => false;

        public void Add(string key, ValueContainer value)
        {
            AssertHasParent();
            underlying[key] = value;
            value.Parent = parent;
        }

        public void Add(KeyValuePair<string, ValueContainer> item)
        {
            AssertHasParent();
            underlying[item.Key] = item.Value;
            item.Value.Parent = parent;

        }

        public void Clear()
        {
            AssertHasParent();
            foreach (var key in underlying.Keys)
            {
                underlying[key].Set((string)null);
            }
        }

        public bool ContainsKey(string key)
        {
            AssertHasParent();
            return underlying.ContainsKey(key);
        }

        public IEnumerator<KeyValuePair<string, ValueContainer>> GetEnumerator()
        {
            AssertHasParent();
            return underlying.GetEnumerator();
        }

        public bool Remove(string key)
        {
            AssertHasParent();
            ValueContainer previous;
            underlying.TryGetValue(key, out previous);
            var removed = underlying.Remove(key);
            if (removed)
            {
                parent.NotifyImmediately(ChildValueChangedEvent.EventName, parent, previous != null ? previous.ValueAsRaw() : null);
            }
            return removed;
        }

        public bool Remove(KeyValuePair<string, ValueContainer> item)
        {
            throw new System.NotImplementedException();
        }

        public bool TryGetValue(string key, out ValueContainer value)
        {
            AssertHasParent();
            return underlying.TryGetValue(key, out value);
        }

        public override bool Equals(object obj)
        {
            return obj is ParentNotifyingDictionary dictionary &&
                   EqualityComparer<string>.Default.Equals(parent.Path, dictionary.parent.Path) &&
                   EqualityComparer<IDictionary<string, ValueContainer>>.Default.Equals(underlying, dictionary.underlying) &&
                   EqualityComparer<List<Action<object>>>.Default.Equals(listeners, dictionary.listeners);
        }

        internal void SetParent(ValueContainer valueContainer)
        {
            parent = valueContainer;
            var keys = new List<string>(underlying.Keys);
            if (parent.Path != null)
            {
                if (valueContainer.Id != null) // If we don't have an ID so there's no point to updating paths now.
                {
                    foreach (var key in keys)
                    {
                        underlying[key].Parent = parent;
                    }
                }
            }
        }

        public bool Contains(KeyValuePair<string, ValueContainer> item)
        {
            return ((ICollection<KeyValuePair<string, ValueContainer>>)underlying).Contains(item);
        }

        public void CopyTo(KeyValuePair<string, ValueContainer>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<string, ValueContainer>>)underlying).CopyTo(array, arrayIndex);
        }

        private void AssertHasParent()
        {
            if (parent == null)
            {
                throw new InvalidOperationException("Can only be use while contained by a ValueContainer instance.");
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)underlying).GetEnumerator();
        }
    }
}