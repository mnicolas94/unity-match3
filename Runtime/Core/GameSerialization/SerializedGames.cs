using System.Collections.Generic;
using UnityEngine;
using Utils;
using Utils.Attributes;

namespace Match3.Core.GameSerialization
{
    [CreateAssetMenu(fileName = "SerializedGames", menuName = "Match3/Persistent/SerializedGames", order = 0)]
    public class SerializedGames : ScriptableObjectSingleton<SerializedGames>
    {
        [SerializeField] private int _maxCount;
        [SerializeField, ToStringLabel] private List<SerializableGame> _games = new List<SerializableGame>();

        public void Add(SerializableGame item)
        {
            if (_games.Count < _maxCount)
                _games.Add(item);
        }

        public void Clear()
        {
            _games.Clear();
        }

        public bool Contains(SerializableGame item)
        {
            return _games.Contains(item);
        }

        public bool Remove(SerializableGame item)
        {
            return _games.Remove(item);
        }

        public int Count => _games.Count;
    }
}