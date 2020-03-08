using System.Collections;
using System.Collections.Generic;

namespace IdleFramework
{
    public class ReadOnlyDictionary<T1, T2> : IDictionary<T1, T2>
    {
        private readonly IDictionary<T1, T2> wrapped;

        public ReadOnlyDictionary(IDictionary<T1, T2> wrapped)
        {
            this.wrapped = wrapped;
        }

        public T2 this[T1 key] { get => wrapped[key]; set => throw new System.InvalidOperationException("Read-only"); }

        public ICollection<T1> Keys => wrapped.Keys;

        public ICollection<T2> Values => wrapped.Values;

        public int Count => wrapped.Count;

        public bool IsReadOnly => true;

        public void Add(T1 key, T2 value)
        {
            throw new System.InvalidOperationException("Read-only");
        }

        public void Add(KeyValuePair<T1, T2> item)
        {
            throw new System.InvalidOperationException("Read-only");
        }

        public void Clear()
        {
            throw new System.InvalidOperationException("Read-only");
        }

        public bool Contains(KeyValuePair<T1, T2> item)
        {
            throw new System.NotImplementedException();
        }

        public bool ContainsKey(T1 key)
        {
            return wrapped.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<T1, T2>[] array, int arrayIndex)
        {
            wrapped.CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<T1, T2>> GetEnumerator()
        {
            return wrapped.GetEnumerator();
        }

        public bool Remove(T1 key)
        {
            throw new System.InvalidOperationException("Read-only");
        }

        public bool Remove(KeyValuePair<T1, T2> item)
        {
            throw new System.InvalidOperationException("Read-only");
        }

        public bool TryGetValue(T1 key, out T2 value)
        {
            value = wrapped.ContainsKey(key) ? wrapped[key] : default(T2);
            return value != null;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return wrapped.GetEnumerator();
        }
    }
}