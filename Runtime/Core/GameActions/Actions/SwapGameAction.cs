using System;
using System.Collections.Generic;
using System.Linq;
using Match3.Core.GameActions.Interactions;
using Match3.Core.Gravity;
using Match3.Core.TokensEvents.Events;
using Match3.Core.TokensEvents.Outputs;
using Match3.Core.TurnSteps;
using UnityEngine;
using Utils.Extensions;

namespace Match3.Core.GameActions.Actions
{
    [Serializable]
    public class SwapGameAction : GameActionBase<SwapInteraction>
    {
        [SerializeField] private bool _consumesTurn;

        public SwapGameAction(bool consumesTurn)
        {
            _consumesTurn = consumesTurn;
        }
        
        public SwapGameAction() : this(true)
        {
        }

        public override bool IsInteractionValid(Board board, SwapInteraction interaction)
        {
            var firstPosition = interaction.FirstPosition;
            var secondPosition = interaction.SecondPosition;
            bool valid = true;
            // check if tokens exist
            bool existsFirstPosition = board.MainLayer.ExistsTokenAt(firstPosition);
            bool existsSecondPosition = board.MainLayer.ExistsTokenAt(secondPosition);
            if (!existsFirstPosition || !existsSecondPosition)
            {
                valid = false;
            }

            // check if are adjacent
            bool adjacent = Board.ArePositionsAdjacent(firstPosition, secondPosition);
            if (!adjacent)
            {
                valid = false;
            }

            return valid;
        }

        public override GameActionExecution Execute(GameContext context, Board board, SwapInteraction interaction)
        {
            List<IEnumerable<TurnStep>> enumerables = new List<IEnumerable<TurnStep>>();
            
            var firstPosition = interaction.FirstPosition;
            var secondPosition = interaction.SecondPosition;
            
            var firstToken = board.MainLayer.GetTokenAt(firstPosition);
            var secondToken = board.MainLayer.GetTokenAt(secondPosition);
            
            // change positions
            board.SwapPositions(firstPosition, secondPosition);
            
            var swapStep = new TurnStepSwap
            (
                firstToken,
                secondToken,
                firstPosition,
                secondPosition
            );

            // check if results in a match
            bool isMatchA = board.ExistsMatchInPosition(context, firstPosition);
            bool isMatchB = board.ExistsMatchInPosition(context, secondPosition);
            bool aRespondsToSwap = firstToken.TokenData.Resolvers.HasResolvers<EventSwapped>();
            bool bRespondsToSwap = secondToken.TokenData.Resolvers.HasResolvers<EventSwapped>();
            bool anyRespondSwap = aRespondsToSwap || bRespondsToSwap;

            bool countAsTurn = isMatchA || isMatchB || anyRespondSwap;
            if (countAsTurn)
            {
                enumerables.Add(new List<TurnStep>{swapStep});
                
                // send swap events
                var swapOutputs = new List<TokenEventOutput>();
                if (aRespondsToSwap)
                {
                    SendEventUtils.SendSwapEvent(board, secondPosition, firstToken, firstPosition, secondToken, swapOutputs);
                }
                if (bRespondsToSwap)
                {
                    SendEventUtils.SendSwapEvent(board, firstPosition, secondToken, secondPosition, firstToken, swapOutputs);
                }
                // handle matches
                if (isMatchA || isMatchB)
                {
                    var matches = board.GetMatches(context);

                    Vector2Int GetPowerUpSpawnPosition(IList<Vector2Int> positions)
                    {
                        if (positions.Contains(firstPosition))
                            return firstPosition;
                        
                        if (positions.Contains(secondPosition))
                            return secondPosition;
                        
                        return positions.GetRandom();
                    }
                    
                    var matchTurnSteps = BoardGameActions.Gts_Matches(context, board, matches, GetPowerUpSpawnPosition, swapOutputs);
                    enumerables.Add(matchTurnSteps);
                }
                else if (swapOutputs.Count > 0)
                {
                    var turnStepsSwap = BoardGameActions.Gts_HandleEventOutputs(context, board, swapOutputs);
                    enumerables.Add(turnStepsSwap);
                }
            }
            else
            {
                // Swap back
                board.SwapPositions(firstPosition, secondPosition);
                var swapBackStep = new TurnStepSwap
                (
                    firstToken,
                    secondToken,
                    secondPosition,
                    firstPosition
                );

                enumerables.Add(new List<TurnStep>
                {
                    swapStep,
                    swapBackStep
                });
            }

            var turnSteps = enumerables.SelectMany(e => e);
            var execution = new GameActionExecution(turnSteps, countAsTurn && _consumesTurn);
            return execution;
        }
    }
}