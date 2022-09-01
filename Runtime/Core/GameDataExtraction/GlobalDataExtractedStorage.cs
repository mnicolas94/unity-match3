using UnityEngine;

namespace Match3.Core.GameDataExtraction
{
    [CreateAssetMenu(
        fileName = "GlobalDataExtractedStorage",
        menuName = "Facticus/Match3/DataExtraction/GlobalDataExtractedStorage",
        order = 0)]
    public class GlobalDataExtractedStorage : ScriptableObject
    {
        [SerializeField] private int _gamesStarted;
        public int GamesStarted => _gamesStarted;
        public void AddGamesStarted() {_gamesStarted++;}

        [SerializeField] private int _gamesCompleted;
        public int GamesCompleted => _gamesCompleted;
        public void AddGamesCompleted() {_gamesCompleted++;}
        
        [SerializeField] private int _gamesWon;
        public int GamesWon => _gamesWon;
        public void AddGamesWon() {_gamesWon++;}
        
        public int GamesLost => _gamesCompleted - _gamesWon;
        
        [SerializeField] private GameExtractedData _data;
        public GameExtractedData Data => _data;

        public static GlobalDataExtractedStorage CreateEmpty()
        {
            var storage = CreateInstance<GlobalDataExtractedStorage>();
            storage._data = new GameExtractedData();
            return storage;
        }
        
        public void AggregateData(GameExtractedData data)
        {
            _data.AggregateData(data);
        }
        
        
    }
}