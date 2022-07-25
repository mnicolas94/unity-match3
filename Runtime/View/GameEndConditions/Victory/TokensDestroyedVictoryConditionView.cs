using System;
using System.Collections.Generic;
using Match3.Core;
using Match3.Core.GameDataExtraction;
using Match3.Core.GameEndConditions.Victory;
using UnityEngine;

namespace Match3.View.GameEndConditions.Victory
{
    public class TokensDestroyedVictoryConditionView : VictoryConditionView<TokensDestroyedVictoryEvaluator>
    {
        [SerializeField] private Transform container;
        [SerializeField] private TokensDestroyedView tokensDestroyedPrefab;

        private Dictionary<TokenData, TokensDestroyedView> _dataViewMap;
        
        protected override void SetupUi(TokensDestroyedVictoryEvaluator victoryEvaluator)
        {
            _dataViewMap = new Dictionary<TokenData, TokensDestroyedView>();
            foreach (var tokenData in victoryEvaluator.Tokens)
            {
                int countRequirement = victoryEvaluator.GetCountRequirement(tokenData);
                AddTokenView(tokenData, countRequirement);
            }
        }

        protected override void UpdateUi(TokensDestroyedVictoryEvaluator victoryEvaluator, GameData gameData)
        {
            var remaining = victoryEvaluator.GetRemainingTokens(gameData);
            foreach (var (tokenData, remainingCount) in remaining)
            {
                var view = GetView(tokenData);
                view.UpdateUi(remainingCount);
            }
        }

        private void AddTokenView(TokenData tokenData, int countRequirement)
        {
            var view = Instantiate(tokensDestroyedPrefab, container);
            int index = _dataViewMap.Count;
            view.transform.SetSiblingIndex(index);
            view.SetupUi(tokenData, countRequirement);
            _dataViewMap.Add(tokenData, view);
        }

        private TokensDestroyedView GetView(TokenData tokenData)
        {
            return _dataViewMap[tokenData];
        }
    }
}