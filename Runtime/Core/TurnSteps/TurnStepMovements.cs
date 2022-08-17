using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Match3.Core.SerializableTuples;
using UnityEngine;

namespace Match3.Core.TurnSteps
{
    [Serializable]
    public class TurnStepMovements : TurnStep
    {
        [SerializeField] private List<MovementsList> _tokensPaths;
        public ReadOnlyCollection<MovementsList> TokensPaths => _tokensPaths.AsReadOnly();

        public TurnStepMovements(List<MovementsList> tokensPaths)
        {
            _tokensPaths = tokensPaths;
        }
    }

    [Serializable]
    public class MovementsList : IList<TokenMovement>
    {
        [SerializeField] private List<TokenMovement> _movements = new List<TokenMovement>();

        public static implicit operator List<TokenMovement>(MovementsList list)
        {
            return list._movements;
        }
        
        public static implicit operator MovementsList(List<TokenMovement> list)
        {
            var mlist = new MovementsList();
            mlist._movements = list;
            return mlist;
        }
        
        public IEnumerator<TokenMovement> GetEnumerator()
        {
            return _movements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) this).GetEnumerator();
        }

        public void Add(TokenMovement item)
        {
            _movements.Add(item);
        }

        public void Clear()
        {
            _movements.Clear();
        }

        public bool Contains(TokenMovement item)
        {
            return _movements.Contains(item);
        }

        public void CopyTo(TokenMovement[] array, int arrayIndex)
        {
            _movements.CopyTo(array, arrayIndex);
        }

        public bool Remove(TokenMovement item)
        {
            return _movements.Remove(item);
        }

        public int Count => _movements.Count;

        public bool IsReadOnly => ((ICollection<TokenMovement>) this).IsReadOnly;

        public int IndexOf(TokenMovement item)
        {
            return _movements.IndexOf(item);
        }

        public void Insert(int index, TokenMovement item)
        {
            _movements.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _movements.RemoveAt(index);
        }

        public TokenMovement this[int index]
        {
            get => _movements[index];
            set => _movements[index] = value;
        }
    }
}