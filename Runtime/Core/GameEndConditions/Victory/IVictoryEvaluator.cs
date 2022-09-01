using System.Collections.Generic;
using Match3.Core.GameDataExtraction;
using Match3.Core.TurnSteps;

namespace Match3.Core.GameEndConditions.Victory
{
    public interface IVictoryEvaluator
    {
        void Initialize(GameController gameController);
        bool CheckVictoryInTurnStep(TurnStep turnStep);
    }
}