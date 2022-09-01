using System;
using UnityEngine;

namespace Match3.Core.GameDataExtraction
{
    [Serializable]
    public class GameData
    {
        [SerializeField] public int TurnCount;
        [SerializeField] public GameExtractedData AllTurnsData;
        [SerializeField] public GameExtractedData LastTurnData;

        public GameData()
        {
            TurnCount = 0;
            AllTurnsData = new GameExtractedData();
            LastTurnData = new GameExtractedData();
        }
    }
}