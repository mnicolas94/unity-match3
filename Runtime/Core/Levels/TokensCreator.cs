using System.Collections.Generic;
using System.Linq;
using Match3.Core.SerializableTuples;
using UnityEngine;

namespace Match3.Core.Levels
{
    public class TokensCreator
    {
        private TokensCreationData _data;
        private List<TokenDataProbability> _aggregatedProbabilities;
        private List<TokenCreationCondition> _completedRequests;
        
        public TokensCreator(TokensCreationData data)
        {
            _data = data;
            _aggregatedProbabilities = GetTokensAggregatedProbabilities(data.Tokens, data.TokensProbabilities);
            _completedRequests = new List<TokenCreationCondition>();
        }
        
        public List<TokenData> AvailableTokens()
        {
            return _aggregatedProbabilities.GetTokens();
        }

        public TokenData SpawnToken(GameController gameController, TokenSource tokenSource, Vector2Int position)
        {
            bool Predicate(TokenCreationCondition req)
            {
                return req.IsMet(gameController, tokenSource, position) && !_completedRequests.Contains(req);
            }

            if (_data.Requests.Any(Predicate))
            {
                var request = _data.Requests.First(Predicate);
                var token = request.CreateToken(gameController, tokenSource, position);
                _completedRequests.Add(request);
                
                return token;
            }
            
            return _aggregatedProbabilities.GetRandomTokenData();
        }
        
        private List<TokenDataProbability> GetTokensProbabilities(
            IList<TokenData> tokens,
            IList<TokenDataProbability> tokensProbabilities)
        {
            var probabilities = new List<TokenDataProbability>();
            float sum = 0;
            foreach (var (tokenData, probability) in tokensProbabilities)
            {
                sum += probability;
                probabilities.Add(new TokenDataProbability(tokenData, probability));
            }

            float remaining = 1 - sum;
            foreach (var tokenData in tokens)
            {
                float prob = remaining / tokens.Count;
                probabilities.Add(new TokenDataProbability(tokenData, prob));
            }

            return probabilities;
        }
        
        private List<TokenDataProbability> GetTokensAggregatedProbabilities(
            IList<TokenData> tokens,
            IList<TokenDataProbability> tokensProbabilities)
        {
            var aggregatedProbabilities = GetTokensProbabilities(tokens, tokensProbabilities);
            float sum = 0;

            foreach (var tokenDataProbability in aggregatedProbabilities)
            {
                var probability = tokenDataProbability.Probability;
                sum += probability;
                tokenDataProbability.Probability = sum;
            }

            return aggregatedProbabilities;
        }
    }
}