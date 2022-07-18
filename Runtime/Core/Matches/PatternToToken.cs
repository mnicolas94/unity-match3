using System;
using System.Collections.Generic;
using Match3.Core.Matches.Patterns;
using Match3.Core.Matches.TokenProviders;
using NaughtyAttributes;
using UnityEngine;

namespace Match3.Core.Matches
{
    [Serializable]
    public class PatternToToken
    {
        [Required] public MatchPatternRecognizerBase PatternRecognizer;
        [SerializeReference, SubclassSelector] private ITokenProvider _tokenProvider;
        
        public TokenData Token => _tokenProvider.GetToken();

        public void Deconstruct(out MatchPatternRecognizerBase patternRecognizer, out ITokenProvider tokenProvider)
        {
            patternRecognizer = PatternRecognizer;
            tokenProvider = _tokenProvider;
        }

        public override string ToString()
        {
            return $"{PatternRecognizer.name} -> {_tokenProvider}";
        }
    }
    
    public static class PatternRecognizersListExtensions
    {
        public static (PatternToToken, bool) FirstPatternMet(this IList<PatternToToken> recognizers, Match match)
        {
            foreach (var tuple in recognizers)
            {
                var patternRecognizer = tuple.PatternRecognizer;
                bool meetsPattern = patternRecognizer.MeetsPattern(match);
                if (meetsPattern)
                {
                    return (tuple, true);
                }
            }

            return (null, false);
        }
    }
}