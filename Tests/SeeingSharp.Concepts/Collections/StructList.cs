using System;
using System.Collections;
using System.Collections.Generic;
using SeeingSharp.Checking;

namespace SeeingSharp.Concepts.Collections
{
    public class StructList<T> : IList<T>, IReadOnlyList<T>
        where T : struct
    {
        private const int DEFAULT_CAPACITY = 4;

        private T[] m_items;
        private int m_version;

        /// <summary>
        /// Creates a new List specialized for structures. 
        /// </summary>
        public StructList(int capacity = DEFAULT_CAPACITY)
        {
            capacity.EnsurePositive(nameof(capacity));

            m_items = new T[capacity];
        }

        public void AddRange(IEnumerable<T> collection)
        {
            InsertRange(Count, collection);
        }

        public void CopyTo(T[] array)
        {
            CopyTo(array, 0);
        }

        public void CopyTo(int index, T[] array, int arrayIndex, int count)
        {
            array.EnsureNotNull(nameof(array));

            Array.Copy(m_items, index, array, arrayIndex, count);
        }

        public bool Exists(Predicate<T> match)
        {
            return FindIndex(match) != -1;
        }

        public T Find(Predicate<T> match)
        {
            match.EnsureNotNull(nameof(match));

            for (var i = 0; i < Count; i++)
            {
                if (match(m_items[i]))
                {
                    return m_items[i];
                }
            }
            return default(T);
        }

        public List<T> FindAll(Predicate<T> match)
        {
            match.EnsureNotNull(nameof(match));
            var list = new List<T>();
            for (var i = 0; i < Count; i++)
            {
                if (match(m_items[i]))
                {
                    list.Add(m_items[i]);
                }
            }
            return list;
        }

        public int FindIndex(Predicate<T> match)
        {
            return FindIndex(0, Count, match);
        }

        public int FindIndex(int startIndex, Predicate<T> match)
        {
            return FindIndex(startIndex, Count - startIndex, match);
        }

        public int FindIndex(int startIndex, int count, Predicate<T> match)
        {
            startIndex.EnsureInRange(0, Count - 1, nameof(startIndex));
            count.EnsurePositive(nameof(count));
            match.EnsureNotNull(nameof(match));

            var num = startIndex + count;
            for (var i = startIndex; i < num; i++)
            {
                if (match(m_items[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        public T FindLast(Predicate<T> match)
        {
            match.EnsureNotNull(nameof(match));

            for (var i = Count - 1; i >= 0; i--)
            {
                if (match(m_items[i]))
                {
                    return m_items[i];
                }
            }
            return default(T);
        }

        public int FindLastIndex(Predicate<T> match)
        {
            return FindLastIndex(Count - 1, Count, match);
        }

        public int FindLastIndex(int startIndex, Predicate<T> match)
        {
            return FindLastIndex(startIndex, startIndex + 1, match);
        }

        public int FindLastIndex(int startIndex, int count, Predicate<T> match)
        {
            startIndex.EnsurePositive(nameof(startIndex));
            count.EnsurePositive(nameof(count));
            match.EnsureNotNull(nameof(match));
            (startIndex + count).EnsureInRange(1, Count, $"{nameof(startIndex)} + {nameof(count)}");

            var num = startIndex - count;
            for (var i = startIndex; i > num; i--)
            {
                if (match(m_items[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        public void ForEach(Action<T> action)
        {
            action.EnsureNotNull(nameof(action));

            var version = m_version;
            var num = 0;
            while (num < Count &&
                   version == m_version)
            {
                action(m_items[num]);
                num++;
            }
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        public StructList<T> GetRange(int index, int count)
        {
            index.EnsurePositive(nameof(index));
            count.EnsurePositiveAndNotZero(nameof(count));
            (index + count).EnsureInRange(1, Count, $"{nameof(index)} + {nameof(count)}");

            var list = new StructList<T>(count);
            Array.Copy(m_items, index, list.m_items, 0, count);
            list.Count = count;
            return list;
        }

        public ref T GetReferenceTo(int index)
        {
            index.EnsureInRange(0, Count - 1, nameof(index));

            return ref m_items[index];
        }

        public int IndexOf(T item, int index)
        {
            index.EnsureInRange(0, Count - 1, nameof(index));
            return Array.IndexOf(m_items, item, index, Count - index);
        }

        public int IndexOf(T item, int index, int count)
        {
            index.EnsureInRange(0, Count - 1, nameof(index));
            (index + count).EnsureInRange(1, Count, $"{nameof(index)} + {nameof(count)}");

            return Array.IndexOf(m_items, item, index, count);
        }

        public void InsertRange(int index, IEnumerable<T> collection)
        {
            collection.EnsureNotNull(nameof(collection));
            index.EnsureInRange(0, Count, nameof(index));

            var collection2 = collection as ICollection<T>;
            if (collection2 != null)
            {
                var count = collection2.Count;
                if (count > 0)
                {
                    EnsureCapacity(Count + count);
                    if (index < Count)
                    {
                        Array.Copy(m_items, index, m_items, index + count, Count - index);
                    }
                    if (ReferenceEquals(this, collection2))
                    {
                        Array.Copy(m_items, 0, m_items, index, index);
                        Array.Copy(m_items, index + count, m_items, index * 2, Count - index);
                    }
                    else
                    {
                        var array = new T[count];
                        collection2.CopyTo(array, 0);
                        array.CopyTo(m_items, index);
                    }
                    Count += count;
                }
            }
            else
            {
                using (var enumerator = collection.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        Insert(index++, enumerator.Current);
                    }
                }
            }
            m_version++;
        }

        public int LastIndexOf(T item)
        {
            if (Count == 0)
            {
                return -1;
            }
            return LastIndexOf(item, Count - 1, Count);
        }

        public int LastIndexOf(T item, int index)
        {
            index.EnsureInRange(0, Count - 1, nameof(index));
            return LastIndexOf(item, index, index + 1);
        }

        public int LastIndexOf(T item, int index, int count)
        {
            index.EnsureInRange(0, Count - 1, nameof(index));
            count.EnsureInRange(0, index + 1, nameof(count));

            if (Count == 0)
            {
                return -1;
            }

            return Array.LastIndexOf(m_items, item, index, count);
        }

        public int RemoveAll(Predicate<T> match)
        {
            match.EnsureNotNull(nameof(match));

            var num = 0;
            while (num < Count && !match(m_items[num]))
            {
                num++;
            }
            if (num >= Count)
            {
                return 0;
            }
            var i = num + 1;
            while (i < Count)
            {
                while (i < Count && match(m_items[i]))
                {
                    i++;
                }
                if (i < Count)
                {
                    m_items[num++] = m_items[i++];
                }
            }
            Array.Clear(m_items, num, Count - num);
            var result = Count - num;
            Count = num;
            m_version++;
            return result;
        }

        public void RemoveRange(int index, int count)
        {
            index.EnsurePositive(nameof(index));
            count.EnsurePositive(nameof(count));
            (index + count).EnsureInRange(1, Count, $"{nameof(index)} + {nameof(count)}");

            if (count <= 0)
            {
                return;
            }

            Count -= count;
            if (index < Count)
            {
                Array.Copy(m_items, index + count, m_items, index, Count - index);
            }
            Array.Clear(m_items, Count, count);
            m_version++;
        }

        public void Reverse()
        {
            Reverse(0, Count);
        }

        public void Reverse(int index, int count)
        {
            index.EnsurePositive(nameof(index));
            count.EnsurePositive(nameof(count));
            (index + count).EnsureInRange(1, Count, $"{nameof(index)} + {nameof(count)}");

            Array.Reverse(m_items, index, count);
            m_version++;
        }

        public void Sort()
        {
            Sort(0, Count, null);
        }

        public void Sort(IComparer<T> comparer)
        {
            Sort(0, Count, comparer);
        }

        public void Sort(int index, int count, IComparer<T> comparer)
        {
            comparer.EnsureNotNull(nameof(comparer));
            index.EnsurePositive(nameof(index));
            count.EnsurePositive(nameof(count));
            (index + count).EnsureInRange(1, Count, $"{nameof(index)} + {nameof(count)}");

            Array.Sort(m_items, index, count, comparer);
            m_version++;
        }

        //public void Sort(Comparison<T> comparison)
        //{
        //    comparison.EnsureNotNull(nameof(comparison));

        //    if (this.m_size > 0)
        //    {
        //        IComparer<T> comparer = new Array.FunctorComparer<T>(comparison);
        //        Array.Sort<T>(this.m_items, 0, this.m_size, comparer);
        //    }
        //}

        public T[] ToArray()
        {
            var array = new T[Count];
            Array.Copy(m_items, 0, array, 0, Count);
            return array;
        }

        public void TrimExcess()
        {
            var num = (int) (m_items.Length * 0.9);
            if (Count < num)
            {
                Capacity = Count;
            }
        }

        public bool TrueForAll(Predicate<T> match)
        {
            match.EnsureNotNull(nameof(match));

            for (var i = 0; i < Count; i++)
            {
                if (!match(m_items[i]))
                {
                    return false;
                }
            }
            return true;
        }

        private void EnsureCapacity(int min)
        {
            if (m_items.Length < min)
            {
                var num = m_items.Length == 0 ? 4 : m_items.Length * 2;
                if (num > 2146435071)
                {
                    num = 2146435071;
                }
                if (num < min)
                {
                    num = min;
                }
                Capacity = num;
            }
        }

        public int Count { get; private set; }

        bool ICollection<T>.IsReadOnly => false;

        public T this[int index]
        {
            get
            {
                if (index >= Count)
                {
                    throw new IndexOutOfRangeException();
                }
                return m_items[index];
            }
            set
            {
                if (index >= Count)
                {
                    throw new IndexOutOfRangeException();
                }
                m_items[index] = value;
                m_version++;
            }
        }

        public void Add(T item)
        {
            if (Count == m_items.Length)
            {
                EnsureCapacity(Count + 1);
            }

            var cachedItems = m_items;
            var cachedSize = Count;

            Count = cachedSize + 1;
            cachedItems[cachedSize] = item;
            m_version++;
        }

        public void Clear()
        {
            if (Count > 0)
            {
                Array.Clear(m_items, 0, Count);
                Count = 0;
            }
            m_version++;
        }

        public bool Contains(T item)
        {
            var @default = EqualityComparer<T>.Default;
            for (var j = 0; j < Count; j++)
            {
                if (@default.Equals(m_items[j], item))
                {
                    return true;
                }
            }
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Array.Copy(m_items, 0, array, arrayIndex, Count);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        public int IndexOf(T item)
        {
            return Array.IndexOf(m_items, item, 0, Count);
        }

        public void Insert(int index, T item)
        {
            index.EnsureInRange(0, Count - 1, nameof(index));

            if (Count == m_items.Length)
            {
                EnsureCapacity(Count + 1);
            }
            if (index < Count)
            {
                Array.Copy(m_items, index, m_items, index + 1, Count - index);
            }
            m_items[index] = item;
            Count++;
            m_version++;
        }

        public bool Remove(T item)
        {
            var num = IndexOf(item);
            if (num >= 0)
            {
                RemoveAt(num);
                return true;
            }
            return false;
        }

        public void RemoveAt(int index)
        {
            index.EnsureInRange(0, Count - 1, nameof(index));

            Count--;
            if (index < Count)
            {
                Array.Copy(m_items, index + 1, m_items, index, Count - index);
            }

            m_items[Count] = default(T);
            m_version++;
        }

        public int Capacity
        {
            get => m_items.Length;
            set
            {
                value.EnsureInRange(Count, int.MaxValue, nameof(value));

                if (value == m_items.Length)
                {
                    return;
                }

                if (value == 0)
                {
                    m_items = new T[0];
                }
                else
                {
                    var array = new T[value];
                    if (Count > 0)
                    {
                        Array.Copy(m_items, 0, array, 0, Count);
                    }
                    m_items = array;
                }
            }
        }

        //*********************************************************************
        //*********************************************************************
        //*********************************************************************
        [Serializable]
        public struct Enumerator : IEnumerator<T>
        {
            private readonly StructList<T> m_hostStructList;
            private int m_index;
            private readonly int m_version;

            public T Current { get; private set; }

            object IEnumerator.Current
            {
                get
                {
                    if (m_index == 0 || m_index == m_hostStructList.Count + 1)
                    {
                        throw new InvalidOperationException("Collection has changed since enumerator started!");
                    }
                    return Current;
                }
            }

            internal Enumerator(StructList<T> structList)
            {
                m_hostStructList = structList;
                m_index = 0;
                m_version = structList.m_version;
                Current = default(T);
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                var list = m_hostStructList;
                if (m_version == list.m_version && m_index < list.Count)
                {
                    Current = list.m_items[m_index];
                    m_index++;
                    return true;
                }
                return MoveNextRare();
            }

            private bool MoveNextRare()
            {
                if (m_version != m_hostStructList.m_version)
                {
                    throw new InvalidOperationException("Collection has changed since enumerator started!");
                }
                m_index = m_hostStructList.Count + 1;
                Current = default(T);
                return false;
            }

            void IEnumerator.Reset()
            {
                if (m_version != m_hostStructList.m_version)
                {
                    throw new InvalidOperationException("Collection has changed since enumerator started!");
                }
                m_index = 0;
                Current = default(T);
            }
        }
    }
}