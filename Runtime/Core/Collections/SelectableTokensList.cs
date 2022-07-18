using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Core.Collections
{
    [Serializable]
    public class SelectableTokensList : IList<TokenData>
    {
        [SerializeField] private List<TokenData> _data;

#region IList implementation

        public IEnumerator<TokenData> GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) this).GetEnumerator();
        }

        public void Add(TokenData item)
        {
            _data.Add(item);
        }

        public void Clear()
        {
            _data.Clear();
        }

        public bool Contains(TokenData item)
        {
            return _data.Contains(item);
        }

        public void CopyTo(TokenData[] array, int arrayIndex)
        {
            _data.CopyTo(array, arrayIndex);
        }

        public bool Remove(TokenData item)
        {
            return _data.Remove(item);
        }

        public int Count => _data.Count;

        public bool IsReadOnly => ((ICollection<TokenData>) this).IsReadOnly;

        public int IndexOf(TokenData item)
        {
            return _data.IndexOf(item);
        }

        public void Insert(int index, TokenData item)
        {
            _data.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _data.RemoveAt(index);
        }

        public TokenData this[int index]
        {
            get => _data[index];
            set => _data[index] = value;
        }

#endregion
    }
}