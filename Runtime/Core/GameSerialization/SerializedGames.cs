using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Utils;
using Utils.Attributes;

namespace Match3.Core.GameSerialization
{
    [CreateAssetMenu(fileName = "SerializedGames", menuName = "Facticus/Match3/Persistent/SerializedGames", order = 0)]
    public class SerializedGames : ScriptableObjectSingleton<SerializedGames>
    {
        [SerializeField] private int _maxCount;
        [SerializeField, ToStringLabel] private List<SerializableGame> _games = new List<SerializableGame>();
        
        public int Count => _games.Count;

        public bool Contains(SerializableGame item)
        {
            return _games.Contains(item);
        }
        
        public void Add(SerializableGame item)
        {
            if (_games.Count < _maxCount)
            {
                _games.Add(item);
            }
        }

        public bool Remove(SerializableGame item)
        {
            bool removed = _games.Remove(item);
            return removed;
        }

        public void Clear()
        {
            _games.Clear();
        }
    }
}