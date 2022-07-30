using System.Collections.Generic;
using Match3.Core.GameDataExtraction;
using Match3.Core.TurnSteps;

namespace Match3.Core.GameEndConditions.Defeat
{
    public interface IDefeatEvaluator
    {
        void Initialize(GameController gameController);
        bool CheckDefeatInTurnStep(TurnStep turnStep, GameData gameData);
    }
}