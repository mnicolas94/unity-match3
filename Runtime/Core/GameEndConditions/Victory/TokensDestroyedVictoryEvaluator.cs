using System;
using System.Collections.Generic;
using Match3.Core.GameDataExtraction;
using Match3.Core.GameDataExtraction.DataExtractors;
using Match3.Core.SerializableDictionaries;
using Match3.Core.TurnSteps;
using Match3.Settings;
using UnityEngine;

namespace Match3.Core.GameEndConditions.Victory
{
    [Serializable]
    public class TokensDestroyedVictoryEvaluator : IVictoryEvaluator, IHomologousTokenReplacer
    {
        [SerializeField] private TokenCountDictionary tokenCounts;
        public List<TokenData> Tokens => new List<TokenData>(tokenCounts.Keys);

        private DataExtractorDestroyedTokens _dataExtractor;
        private DestroyedTokensData _data;
        
        public int GetCountRequirement(TokenData tokenData)
        {
            return tokenCounts[tokenData];
        }

        public void Initialize(GameController gameController)
        {
            _dataExtractor = new DataExtractorDestroyedTokens();
            _data = new DestroyedTokensData();
        }

        public bool CheckVictoryInTurnStep(TurnStep turnStep)
        {
            ExtractData(turnStep);
            foreach (var tokenData in tokenCounts.Keys)
            {
                int countRequirement = GetCountRequirement(tokenData);
                int destroyedCount = _data.GetDestroyedCount(tokenData);
                if (destroyedCount < countRequirement)
                {
                    return false;
                }
            }

            return true;
        }

        public List<(TokenData, int)> GetRemainingTokens()
        {
            var remaining = new List<(TokenData, int)>();
            
            foreach (var tokenData in tokenCounts.Keys)
            {
                int countRequirement = GetCountRequirement(tokenData);
                int remainingCount = countRequirement;
                int destroyedCount = _data.GetDestroyedCount(tokenData);
                remainingCount -= destroyedCount;
                remainingCount = Math.Max(0, remainingCount);
                
                remaining.Add((tokenData, remainingCount));
            }

            return remaining;
        }

        public void ReplaceToken(TokenData toReplace, TokenData replacement)
        {
            if (tokenCounts.ContainsKey(toReplace))
            {
                int count = tokenCounts[toReplace];
                tokenCounts.Remove(toReplace);
                tokenCounts.Add(replacement, count);
            }
        }

        private void ExtractData(TurnStep turnStep)
        {
            bool canExtract = _dataExtractor.CanExtractDataFrom(turnStep);
            if (canExtract)
            {
                var data = _dataExtractor.ExtractData(turnStep);
                _data.Aggregate(data);
            }
        }
    }
}