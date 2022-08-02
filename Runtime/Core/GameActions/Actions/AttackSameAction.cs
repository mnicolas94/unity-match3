using System;
using System.Collections.Generic;
using Match3.Core.GameActions.Interactions;
using Match3.Core.Gravity;
using Match3.Core.SerializableTuples;
using Match3.Core.TurnSteps;
using Match3.Core.TurnSteps.TokensDestructionSource;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using Random = UnityEngine.Random;

namespace Match3.Core.GameActions.Actions
{
    [MovedFrom(false, "BoardCores.Data.GameActions.Actions")]
    [Serializable]
    public class AttackSameAction : GameActionBase<SelectPositionInteraction>
    {
        [SerializeReference, SubclassSelector] private ITokenDamageSource _damageSource;

        public override bool IsInteractionValid(Board board, SelectPositionInteraction interaction)
        {
            var position = interaction.Position;
            bool exists = board.MainLayer.ExistsTokenAt(position);
            if (exists)
            {
                var token = board.MainLayer.GetTokenAt(position);
                bool matchWithItself = token.TokenData.CanMatchWithItself;
                return matchWithItself;
            }

            return false;
        }

        public override GameActionExecution Execute(GameContext context, Board board, SelectPositionInteraction interaction)
        {
            var position = interaction.Position;
            var token = board.MainLayer.GetTokenAt(position);
            var tokenData = token.TokenData;
            var positionsToAttack = board.MainLayer.GetAllPositionsOfTokenData(tokenData);
            var positions = positionsToAttack.ConvertAll(pos => new PositionToAttackOrder(pos, Random.Range(0, 5)));
            var destroyedTokens = BoardGameActions.T_AttackPositions(context, board, positions);
            var destruction = new TokensDamaged(_damageSource, position, destroyedTokens);
            var destructions = new List<TokensDamaged> {destruction};
            var turnSteps = BoardGameActions.Gts_HandleTokensDestruction(context, board, destructions);
            var execution = new GameActionExecution(turnSteps, countAsTurn: false);
            return execution;
        }
    }
}