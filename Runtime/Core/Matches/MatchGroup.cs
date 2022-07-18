using System.Collections.Generic;
using System.Collections.ObjectModel;
using NaughtyAttributes;
using UnityEngine;

namespace Match3.Core.Matches
{
    [CreateAssetMenu(fileName = "MatchGroup", menuName = "Facticus/Match3/MatchGroup")]
    public class MatchGroup : ScriptableObject
    {
        [SerializeField, ValidateInput(nameof(IsNotEmpty), "tokens should not be empty")]
         private List<TokenData> _tokens;
         
        [SerializeField]
         private List<PatternToToken> _matchPatternsToTokens;

        public ReadOnlyCollection<TokenData> Tokens => _tokens.AsReadOnly();
        public ReadOnlyCollection<PatternToToken> MatchPatternsToTokens => _matchPatternsToTokens.AsReadOnly();

        private bool IsNotEmpty()
        {
            return _tokens.Count > 0;
        }
    }
}