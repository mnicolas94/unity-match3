using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Match3.Core;
using UnityEngine;
using Utils;
using Utils.Attributes;

namespace Match3.Settings
{
    [CreateAssetMenu(fileName = "HomologousTokensGroups", menuName = "Facticus/Match3/Settings/HomologousTokensGroups", order = 0)]
    public class HomologousTokensGroups : ScriptableObjectSingleton<HomologousTokensGroups>
    {
        [SerializeField, ToStringLabel] private List<HomologousTokensGroup> _groups;

        public bool ExistGroupForToken(TokenData token)
        {
            return _groups.Exists(group => group.Tokens.Contains(token));
        }
        
        public HomologousTokensGroup GetTokenGroup(TokenData token)
        {
            return _groups.Find(group => group.Tokens.Contains(token));
        }

        public bool AreHomologous(TokenData tokenA, TokenData tokenB)
        {
            bool bothHaveGroup = ExistGroupForToken(tokenA) && ExistGroupForToken(tokenB);
            if (bothHaveGroup)
            {
                var groupA = GetTokenGroup(tokenA);
                var groupB = GetTokenGroup(tokenB);
                return groupA == groupB;
            }

            return false;
        }
    }

    [Serializable]
    public class HomologousTokensGroup
    {
        [SerializeField] private List<TokenData> _tokens;

        public ReadOnlyCollection<TokenData> Tokens => _tokens.AsReadOnly();

        public override string ToString()
        {
            return string.Join(" ", _tokens.ConvertAll(tk => tk.name.Replace("tk-resource-", "")));
        }
    }
}