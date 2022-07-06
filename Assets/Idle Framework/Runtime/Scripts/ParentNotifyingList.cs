using io.github.thisisnozaku.idle.framework;
using io.github.thisisnozaku.idle.framework.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Engine
{
    public class ParentNotifyingList : IList<ValueContainer>
    {
        private ValueContainer parent;
        private IList<ValueContainer> underlying;

        public int Count => ((ICollection<ValueContainer>)underlying).Count;

        public bool IsReadOnly => ((ICollection<ValueContainer>)underlying).IsReadOnly;

        public ParentNotifyingList(ValueContainer parent, IList<ValueContainer> other)
        {
            if (parent == null)
            {
                throw new ArgumentNullException("parent");
            }
            this.parent = parent;
            this.underlying = other != null ? new List<ValueContainer>(other) : new List<ValueContainer>();

            var children = new List<ValueContainer>(underlying);
            for (int i = 0; i < children.Count; i++)
            {
                children[i].Parent = parent;
            }
        }

        public ValueContainer this[int index]
        {
            get
            {
                return underlying[index];
            }
            set
            {
                ValueContainer previous = null;
                if (underlying.Count > index)
                {
                    previous = underlying[index];
                }
                underlying[index] = value;
                value.Parent = parent;

                NotifyParentOfChildChange(string.Join(".", parent.Path, index), previous != null ? previous.ValueAsRaw() : null, value.ValueAsRaw());
            }
        }

        public void Add(ValueContainer item)
        {
            AssertHasParent();
            ((ICollection<ValueContainer>)underlying).Add(item);
            item.Parent = parent;
            if(parent.Path != null)
            {
                item.Path = String.Join(".", parent.Path, underlying.Count - 1);
            }
            NotifyParentOfChildChange(item.Path, null, item.ValueAsRaw());
            //parent.NotifyImmediately(ChildValueChangedEvent.EventName, parent, item.Path, null, item.ValueAsRaw());
            if (parent.Parent != null)
            {
                //parent.Parent.NotifyImmediately(ChildValueChangedEvent.EventName, parent, null, item.ValueAsRaw(), parent.Parent, "set");
            }
        }

        public void Clear()
        {
            AssertHasParent();
            foreach (var item in underlying)
            {
                item.Set((string)null);
            }
        }

        public bool Contains(ValueContainer item)
        {
            AssertHasParent();
            return ((ICollection<ValueContainer>)underlying).Contains(item);
        }

        public void CopyTo(ValueContainer[] array, int arrayIndex)
        {
            AssertHasParent();
            ((ICollection<ValueContainer>)underlying).CopyTo(array, arrayIndex);
        }

        public IEnumerator<ValueContainer> GetEnumerator()
        {
            AssertHasParent();
            return ((IEnumerable<ValueContainer>)underlying).GetEnumerator();
        }

        public int IndexOf(ValueContainer item)
        {
            AssertHasParent();
            return ((IList<ValueContainer>)underlying).IndexOf(item);
        }

        public void Insert(int index, ValueContainer item)
        {
            AssertHasParent();
            var previous = underlying[index];
            ((IList<ValueContainer>)underlying).Insert(index, item);
            NotifyParentOfChildChange(string.Join(".", parent.Parent, index), previous != null ? previous.ValueAsRaw() : null, item.ValueAsRaw());
        }

        public bool Remove(ValueContainer item)
        {
            AssertHasParent();
            bool removed = ((ICollection<ValueContainer>)underlying).Remove(item);
            if (removed)
            {
                NotifyParentOfChildChange(item.Path, item.ValueAsRaw(), null);
                //parent.NotifyImmediately(ChildValueChangedEvent.EventName, parent, item.Path, item, item.ValueAsRaw());
            }
            return removed;
        }

        public void RemoveAt(int index)
        {
            AssertHasParent();
            var previous = underlying[index];
            ((IList<ValueContainer>)underlying).RemoveAt(index);
            NotifyParentOfChildChange(string.Join(".", parent.Parent, index), previous != null ? previous.ValueAsRaw() : null, null);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            AssertHasParent();
            return ((IEnumerable)underlying).GetEnumerator();
        }

        private void AssertHasParent()
        {
            if (parent == null)
            {
                throw new InvalidOperationException("Can only be use while contained by a ValueContainer instance.");
            }
        }

        public void SetParent(ValueContainer parent)
        {
            this.parent = parent;
            if (parent.Path != null)
            {
                for (int i = 0; i < underlying.Count; i++)
                {
                    underlying[i].Path = string.Join(".", parent.Path, i);
                }
            }
        }

        private void NotifyParentOfChildChange(string path, object oldValue, object newValue)
        {
            parent.NotifyImmediately(ChildValueChangedEvent.EventName, new ChildValueChangedEvent(parent, parent, oldValue, newValue, "set"));
        }
    }
}