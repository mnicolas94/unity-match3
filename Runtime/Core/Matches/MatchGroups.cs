using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using Utils.Attributes;

namespace Match3.Core.Matches
{
    [Serializable]
    public class MatchGroups
    {
        [SerializeField] private List<MatchGroup> _groups;
        [SerializeField, ToStringLabel] private List<PatternToToken> _globalMatchPatterns;
        
        [NonSerialized] private Dictionary<TokenData, MatchGroup> _tokenToGroupCache;

        private Dictionary<TokenData, MatchGroup> TokenToGroupCache
        {
            get
            {
                if (_tokenToGroupCache == null)
                {
                    LoadCache();
                }

                return _tokenToGroupCache;
            }
        }

        public ReadOnlyCollection<PatternToToken> GlobalMatchPatterns => _globalMatchPatterns.AsReadOnly();

        public MatchGroups(List<MatchGroup> groups, List<PatternToToken> globalMatchPatterns)
        {
            _groups = groups;
            _globalMatchPatterns = globalMatchPatterns;
            LoadCache();
        }

        public MatchGroups(MatchGroups other)
        {
            _groups = new List<MatchGroup>(other._groups);
            _globalMatchPatterns = new List<PatternToToken>(other._globalMatchPatterns);
            LoadCache();
        }
        
        public MatchGroups() : this(new List<MatchGroup>(), new List<PatternToToken>())
        {
        }
        
        private void LoadCache()
        {
            _tokenToGroupCache = new Dictionary<TokenData, MatchGroup>();
            foreach (var matchGroup in _groups)
            {
                LoadGroupIntoCache(matchGroup);
            }
        }

        private void LoadGroupIntoCache(MatchGroup group)
        {
            foreach (var tokenData in group.Tokens)
            {
                TokenToGroupCache.Add(tokenData, group);
            }
        }
        
        private void UnloadGroupFromCache(MatchGroup group)
        {
            foreach (var tokenData in group.Tokens)
            {
                TokenToGroupCache.Remove(tokenData);
            }
        }

        public bool IsTokenInGroup(TokenData tokenData)
        {
            return TokenToGroupCache.ContainsKey(tokenData);
        }
        
        public (MatchGroup, bool) GetGroupOfToken(TokenData tokenData)
        {
            bool hasGroup = IsTokenInGroup(tokenData);
            MatchGroup group = null;
            if (hasGroup)
                group = TokenToGroupCache[tokenData];

            return (group, hasGroup);
        }

        public bool AreFromSameGroup(Token a, Token b)
        {
            return AreFromSameGroup(a.TokenData, b.TokenData);
        }
        
        public bool AreFromSameGroup(TokenData a, TokenData b)
        {
            var (groupA, aIsInGroup) = GetGroupOfToken(a);
            var (groupB, bIsInGroup) = GetGroupOfToken(b);
            bool bothHaveGroup = aIsInGroup && bIsInGroup;
            bool isSameGroup = groupA == groupB;
            return bothHaveGroup && isSameGroup;
        }
        
        public bool DoesTokensMatch(Token a, Token b)
        {
            if (a != null && b != null)
            {
                bool isSame = a.TokenData == b.TokenData;
                bool selfMatch = a.TokenData.CanMatchWithItself;
                bool sameGroup = AreFromSameGroup(a, b);
                return isSame && selfMatch || sameGroup;
            }

            return false;
        }

        public IEnumerable<TokenData> GetAllMatchesOfToken(Token token)
        {
            return GetAllMatchesOfToken(token.TokenData);
        }

        public IEnumerable<TokenData> GetAllMatchesOfToken(TokenData tokenData)
        {
            var (group, existsGroup) = GetGroupOfToken(tokenData);
            if (!existsGroup) yield break;
            foreach (var otherTokenData in group.Tokens)
            {
                if (otherTokenData != tokenData)
                    yield return otherTokenData;
            }
        }
        
        public (PatternToToken, bool) FirstPatternMet(Board board, Match match)
        {
            var firstPosition = match.Positions[0];
            var firstToken = board.MainLayer.GetTokenAt(firstPosition);
            var tokenData = firstToken.TokenData;
            var (group, hasGroup) = GetGroupOfToken(tokenData);
            if (hasGroup)
            {
                // check if group has a token to spawn when a match pattern exists
                var (patternRecognizer, existsPattern) = group.MatchPatternsToTokens.FirstPatternMet(match);
                if (existsPattern)
                {
                    return (patternRecognizer, true);
                }
            }
            // check if match meets a global pattern that spawns a token
            var (globalPatternRecognizer, globalExistsPattern) = _globalMatchPatterns.FirstPatternMet(match);
            if (globalExistsPattern)
                return (globalPatternRecognizer, true);

            return (null, false);
        }
    }
}