using System;
using System.Collections.Generic;
using Match3.Core.GameDataExtraction;
using Match3.Core.GameDataExtraction.DataExtractors;
using Match3.Core.TurnSteps;
using UnityEngine;

namespace Match3.Core.GameEndConditions.Defeat
{
    [Serializable]
    public class MovementsDefeatEvaluator : IDefeatEvaluator
    {
        [SerializeField] private int _movements;

        public int Movements => _movements;

        public void Initialize(GameController gameController)
        {
            // do nothing
        }

        public IEnumerable<IDataExtractor> GetDataExtractors()
        {
            yield break;
        }

        public bool CheckDefeatInTurnStep(TurnStep turnStep, GameData gameData)
        {
            return GetRemainingMovements(gameData) <= 0;
        }

        public int GetRemainingMovements(GameData gameData)
        {
            int remaining = _movements - gameData.TurnCount;
            remaining = Math.Max(0, remaining);
            return remaining;
        }
    }
}