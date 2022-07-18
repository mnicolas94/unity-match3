using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Match3.Core.SerializableTuples
{
    [Serializable]
    public class TokenDataProbability
    {
        [SerializeField] private TokenData _tokenData;
        [SerializeField] [Range(0.0f, 1.0f)] private float _probability;

        public TokenData TokenData
        {
            get => _tokenData;
            set => _tokenData = value;
        }

        public float Probability
        {
            get => _probability;
            set => _probability = value;
        }

        public TokenDataProbability(TokenData tokenData, float probability)
        {
            _tokenData = tokenData;
            _probability = probability;
        }

        public void Deconstruct(out TokenData tokenData, out float probability)
        {
            tokenData = _tokenData;
            probability = _probability;
        }
    }

    public static class TokenDataProbabilityListExtension
    {
        public static TokenData GetRandomTokenData(this IList<TokenDataProbability> list)
        {
            float randomValue = Random.value;
            foreach (var (tokenData, accumulatedProbability) in list)
            {
                if (randomValue <= accumulatedProbability)
                    return tokenData;
            }
            
            Debug.LogWarning("List of tokens accumulated probabilities is not normalized, i.e last probability is >= 1.0");
            return list[list.Count - 1].TokenData;
        }

        public static List<TokenData> GetTokens(this IList<TokenDataProbability> list)
        {
            var tokens = new List<TokenData>();
            foreach (var (tokenData, probability) in list)
            {
                tokens.Add(tokenData);
            }

            return tokens;
        }
    }
}