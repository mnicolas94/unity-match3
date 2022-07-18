using System;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Core.GameDataExtraction
{
    [Serializable]
    public class GameExtractedData
    {
        [SerializeField] private List<IExtractedData> _data;

        public GameExtractedData()
        {
            _data = new List<IExtractedData>();
        }
        
        public bool HasData<T>() where T : IExtractedData
        {
            return _data.Exists(data => data is T);
        }

        public bool HasData(Type dataType)
        {
            return _data.Exists(data => data.GetType() == dataType);
        }

        public T GetData<T>() where T : IExtractedData
        {
            if (HasData<T>())
                return (T) _data.Find(data => data is T);
            throw new ArgumentException($"The type of data {typeof(T).Name} is not present in the game data");
        }
        
        public IExtractedData GetData(Type dataType)
        {
            if (HasData(dataType))
                return _data.Find(data => data.GetType() == dataType);
            throw new ArgumentException($"The type of data {dataType.Name} is not present in the game data");
        }
        
        public bool TryGetData<T>(out T data) where T : IExtractedData
        {
            if (HasData<T>())
            {
                data = (T) _data.Find(data => data is T);
                return true;
            }

            data = default;
            return false;
        }

        public void AggregateData(IExtractedData otherData)
        {
            var dataType = otherData.GetType();
            if (HasData(dataType))
            {
                var data = GetData(dataType);
                data.Aggregate(otherData);
            }
            else
            {
                _data.Add(otherData.GetClone());
            }
        }

        public void AggregateData(GameExtractedData otherData)
        {
            foreach (var data in otherData._data)
            {
                AggregateData(data);
            }
        }

        public void ClearData()
        {
            _data.Clear();
//            foreach (var gameData in _data)
//            {
//                gameData.Clear();
//            }
        }
    }
}