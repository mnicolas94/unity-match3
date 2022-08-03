using System;
using Match3.Core.SerializableDictionaries;
using Match3.Core.TurnSteps;
using UnityEngine;

namespace Match3.Core.GameDataExtraction.DataExtractors
{
    [Serializable]
    public class DestroyedTokensData : IExtractedData
    {
        [SerializeField] private TokenCountDictionary _destroyedTokens;

        public DestroyedTokensData(TokenCountDictionary destroyedTokens)
        {
            _destroyedTokens = destroyedTokens;
        }

        public bool HasToken(TokenData tokenData)
        {
            return _destroyedTokens.ContainsKey(tokenData);
        }

        public int GetDestroyedCount(TokenData tokenData)
        {
            return _destroyedTokens[tokenData];
        }

        public IExtractedData GetClone()
        {
            var dictCopy = new TokenCountDictionary();
            dictCopy.CopyFrom(_destroyedTokens);
            return new DestroyedTokensData(dictCopy);
        }

        public void Aggregate(IExtractedData other)
        {
            if (other is DestroyedTokensData otherDestroyedTokens)
            {
                foreach (var pair in otherDestroyedTokens._destroyedTokens)
                {
                    var token = pair.Key;
                    var count = pair.Value;
                    if (HasToken(token))
                    {
                        _destroyedTokens[token] += count;
                    }
                    else
                    {
                        _destroyedTokens.Add(token, count);
                    }
                }
            }
        }

        public void Clear()
        {
            _destroyedTokens.Clear();
        }
    }
    
    [Serializable]
    public class DataExtractorDestroyedTokens : DataExtractorBase<TurnStepDamageTokens, DestroyedTokensData>
    {
        public override DestroyedTokensData ExtractData(TurnStepDamageTokens turnStep)
        {
            var destroyedTokens = new TokenCountDictionary();
            foreach (var (position, token) in turnStep.GetAllPositionsTokensDestroyed())
            {
                var tokenData = token.TokenData;
                if (!destroyedTokens.ContainsKey(tokenData))
                {
                    destroyedTokens[tokenData] = 0;
                }
                destroyedTokens[tokenData]++;
            }
            return new DestroyedTokensData(destroyedTokens);
        }
    }
}