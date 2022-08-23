using System.Collections.Generic;
using System.Linq;
using Match3.Core.SerializableTuples;
using UnityEngine;
using Utils;
using Utils.Extensions;

namespace Match3.Core.Levels
{
    public class TokensCreator
    {
        private TokensCreationData _data;
        private List<TokenDataProbability> _aggregatedProbabilities;
        private List<ConditionalTokenCreation> _requests;
        private List<ConditionalTokenCreation> _completedRequests;
        
        public TokensCreator(TokensCreationData data, List<ConditionalTokenCreation> globalRequests)
        {
            _data = data;
            _requests = new List<ConditionalTokenCreation>(globalRequests);
            _requests.AddRangeIfNotExists(data.Requests);
            _aggregatedProbabilities = GetTokensAggregatedProbabilities(data.Tokens, data.TokensProbabilities);
            _completedRequests = new List<ConditionalTokenCreation>();
        }
        
        public List<TokenData> AvailableTokens()
        {
            return _aggregatedProbabilities.GetTokens();
        }

        public TokenData SpawnToken(GameController gameController, TokenSource tokenSource, Vector2Int position)
        {
            bool Predicate(ConditionalTokenCreation req)
            {
                bool isMet = req.IsMet(gameController, tokenSource, position);
                bool alreadyCompleted = _completedRequests.Contains(req);
                return isMet && !alreadyCompleted;
            }

            var request = _requests.FirstOrDefault(Predicate);
            if (request != null)
            {
                var token = request.GetToken(gameController, tokenSource, position);
                if (!request.IsPermanent)
                {
                    _completedRequests.Add(request);
                }
                
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