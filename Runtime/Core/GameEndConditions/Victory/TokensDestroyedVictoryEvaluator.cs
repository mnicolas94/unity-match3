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

        public int GetCountRequirement(TokenData tokenData)
        {
            return tokenCounts[tokenData];
        }

        public void Initialize(GameController gameController)
        {
            // do nothing
        }

        public IEnumerable<IDataExtractor> GetDataExtractors()
        {
            yield return new DataExtractorDestroyedTokens();
        }

        public bool CheckVictoryInTurnStep(TurnStep turnStep, GameData gameData)
        {
            bool hasData = gameData.AllTurnsData.TryGetData<DestroyedTokensData>(out var data);
            if (hasData)
            {
                foreach (var tokenData in tokenCounts.Keys)
                {
                    int countRequirement = GetCountRequirement(tokenData);
                    bool isDestroyed = data.HasToken(tokenData);
                    if (!isDestroyed)
                        return false;
                    int destroyedCount = data.GetDestroyedCount(tokenData);
                    if (destroyedCount < countRequirement)
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        public List<(TokenData, int)> GetRemainingTokens(GameData gameData)
        {
            bool hasData = gameData.AllTurnsData.TryGetData<DestroyedTokensData>(out var data);
            var remaining = new List<(TokenData, int)>();
            
            foreach (var tokenData in tokenCounts.Keys)
            {
                int countRequirement = GetCountRequirement(tokenData);
                int remainingCount = countRequirement;
                if (hasData)
                {
                    bool isDestroyed = data.HasToken(tokenData);
                    if (isDestroyed)
                    {
                        int destroyedCount = data.GetDestroyedCount(tokenData);
                        remainingCount -= destroyedCount;
                        remainingCount = Math.Max(0, remainingCount);
                    }
                }
                
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
    }
}