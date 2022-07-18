using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Core.Collections
{
    [CreateAssetMenu(fileName = "TokensList", menuName = "Match3/Collections/TokensList", order = 0)]
    public class ScriptableTokensList : ScriptableObject, IList<TokenData>
    {
        [SerializeField] private List<TokenData> _tokens;
        
        public IEnumerator<TokenData> GetEnumerator()
        {
            return _tokens.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) this).GetEnumerator();
        }

        public void Add(TokenData item)
        {
            _tokens.Add(item);
        }

        public void Clear()
        {
            _tokens.Clear();
        }

        public bool Contains(TokenData item)
        {
            return _tokens.Contains(item);
        }

        public void CopyTo(TokenData[] array, int arrayIndex)
        {
            _tokens.CopyTo(array, arrayIndex);
        }

        public bool Remove(TokenData item)
        {
            return _tokens.Remove(item);
        }

        public int Count => _tokens.Count;

        public bool IsReadOnly => ((ICollection<TokenData>) this).IsReadOnly;

        public int IndexOf(TokenData item)
        {
            return _tokens.IndexOf(item);
        }

        public void Insert(int index, TokenData item)
        {
            _tokens.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _tokens.RemoveAt(index);
        }

        public TokenData this[int index]
        {
            get => _tokens[index];
            set => _tokens[index] = value;
        }
    }
}