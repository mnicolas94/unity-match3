using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Match3.Core.Collections;
using Match3.Core.SerializableTuples;
using Match3.Settings;
using NaughtyAttributes;
using UnityEngine;
using Utils.Attributes;
using Utils.Extensions;

namespace Match3.Core.Levels
{
    [Serializable]
    public class TokensCreationData : IHomologousTokenReplacer
    {
        /// <summary>
        /// Tokens
        /// </summary>
        [SerializeField, ValidateInput(nameof(ValidateTokensNotEmpty), "Tokens list should not be empty")]
        private SelectableTokensList _tokens;
        public ReadOnlyCollection<TokenData> Tokens => new ReadOnlyCollection<TokenData>(_tokens);
        
        /// <summary>
        /// Tokens probabilities
        /// </summary>
        [SerializeField, ValidateInput(nameof(ValidateProbabilitiesLessThanOne), "Probabilities should sum less or equal to 1.0")]
        private List<TokenDataProbability> _tokensProbabilities;
        public ReadOnlyCollection<TokenDataProbability> TokensProbabilities => _tokensProbabilities.AsReadOnly();

        /// <summary>
        /// Creations requests
        /// </summary>
        [SerializeField, ToStringLabel] private List<ConditionalTokenCreation> _requests;
        public ReadOnlyCollection<ConditionalTokenCreation> Requests => _requests.AsReadOnly();

        public void AddTokenCreationRequest(ConditionalTokenCreation request)
        {
            _requests.Add(request);
        }

        public List<TokenData> GetAllTokens()
        {
            var availableTokens = AvailableTokens();
            foreach (var request in _requests)
            {
                availableTokens.AddRangeIfNotExists(request.GetAvailableTokens());
            }

            return availableTokens;
        }
        
        public List<TokenData> AvailableTokens()
        {
            var tokens = new List<TokenData>();
            tokens.AddRangeIfNotExists(_tokens);
            tokens.AddRangeIfNotExists(_tokensProbabilities.GetTokens());
            return tokens;
        }
        
        public void ReplaceToken(TokenData toReplace, TokenData replacement)
        {
            // tokens
            for (int i = 0; i < _tokens.Count; i++)
            {
                if (_tokens[i] == toReplace)
                    _tokens[i] = replacement;
            }
            
            // tokens probabilities
            for (int i = 0; i < _tokensProbabilities.Count; i++)
            {
                var (token, prob) = _tokensProbabilities[i];
                if (token == toReplace)
                    _tokensProbabilities[i] = new TokenDataProbability(replacement, prob);
            }
            
            // requests
            foreach (var request in _requests)
            {
                request.ReplaceToken(toReplace, replacement);
            }
        }
        
        public bool ValidateData()
        {
            return ValidateTokensNotEmpty() && ValidateProbabilitiesLessThanOne();
        }

        private bool ValidateTokensNotEmpty()
        {
            return _tokens != null && _tokens.Count > 0;
        }
        
        private bool ValidateProbabilitiesLessThanOne()
        {
            float sum = 0;
            foreach (var (_, probability) in _tokensProbabilities)
            {
                sum += probability;
            }

            return sum <= 1.0;
        }
    }
}