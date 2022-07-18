﻿using System.Collections.Generic;
using Match3.Core.GameDataExtraction;
using Match3.Core.TurnSteps;

namespace Match3.Core.GameEndConditions.Victory
{
    public interface IVictoryEvaluator
    {
        IEnumerable<IDataExtractor> GetDataExtractors();
        bool CheckVictoryInTurnStep(TurnStep turnStep, GameData gameData);
    }
}