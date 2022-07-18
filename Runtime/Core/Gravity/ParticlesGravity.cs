using System;
using System.Collections.Generic;
using Match3.Core.SerializableTuples;
using Match3.Core.TurnSteps;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Match3.Core.Gravity
{
    using LongReturnType = ValueTuple<List<MovementsList>, Dictionary<Token, (MovementsList, TokenSource)>>;
    
    [Serializable]
    public class ParticlesGravity : IBoardGravity
    {
        public LongReturnType GetTurnStepMovements(Board board)
        {
            var bounds = board.BoardShape.GetBounds();
            var turnMovements = new List<MovementsList>();
            var fakeMovements = new Dictionary<Token, (MovementsList, TokenSource)>();

            bool keepMoving = true;
            while (keepMoving)
            {
                var movements = new List<TokenMovement>();
                int lastRow = bounds.yMax + 1; // get to tokens sources
                for (int y = bounds.yMin; y < lastRow; y++)
                {
                    for (int x = bounds.xMin; x < bounds.xMax; x++)
                    {
                        var position = new Vector2Int(x, y);
                        bool canMoveFrom = GravityUtils.CanMoveFrom(board, position);

                        if (canMoveFrom)
                        {
                            bool move = false;
                            Vector2Int newPosition = default;
                            float randomNumber = Random.value;
                            var firstSide = randomNumber > 0.5f ? Vector2Int.left : Vector2Int.right;
                            var secondSide = randomNumber > 0.5f ? Vector2Int.right : Vector2Int.left;

                            if (CanMoveBottom(board, position, out var bottomPosition))
                            {
                                newPosition = bottomPosition;
                                move = true;
                            }
                            else if (CanMoveBottomSide(
                                board,
                                position,
                                firstSide,
                                out var bottomFirstSidePosition))
                            {
                                newPosition = bottomFirstSidePosition;
                                move = true;
                            }
                            else if (CanMoveBottomSide(
                                board,
                                position,
                                secondSide,
                                out var bottomSecondSidePosition))
                            {
                                newPosition = bottomSecondSidePosition;
                                move = true;
                            }

                            if (move)
                            {
                                TokenMovement movement;
                                bool hasTokenSource = board.ExistsTokenSourceAt(position);
                                if (hasTokenSource)
                                {
                                    var tokenSource = board.GetTokenSourceAt(position);
                                    var fakeToken = new FakeToken();
                                    board.MainLayer.AddTokenAt(fakeToken, newPosition);
                                    movement = new TokenMovement(fakeToken, position, newPosition);

                                    fakeMovements.Add(fakeToken, (new List<TokenMovement> {movement}, tokenSource));
                                }
                                else
                                {
                                    var token = board.MainLayer.GetTokenAt(position);
                                    board.MoveTokenTo(position, newPosition);
                                    movement = new TokenMovement(token, position, newPosition);
                                    if (fakeMovements.ContainsKey(token))
                                    {
                                        var (moves, source) = fakeMovements[token];
                                        moves.Add(movement);
                                    }
                                }

                                movements.Add(movement);
                            }
                        }
                    }
                }
                
                if (movements.Count > 0)
                {
                    turnMovements.Add(movements);
                }
                else
                {
                    keepMoving = false;
                }
            }
            
            return (turnMovements, fakeMovements);
        }
        
        private static bool CanMoveBottom(Board board, Vector2Int position, out Vector2Int bottomPosition)
        {
            bottomPosition = position + Vector2Int.down;
            return GravityUtils.CanMoveTo(board, bottomPosition);
        }
        
        private static bool CanMoveBottomLeft(Board board, Vector2Int position, out Vector2Int bottomLeftPosition)
        {
            return CanMoveBottomSide(board, position, Vector2Int.left, out bottomLeftPosition);
        }
        
        private static bool CanMoveBottomRight(Board board, Vector2Int position, out Vector2Int bottomRightPosition)
        {
            return CanMoveBottomSide(board, position, Vector2Int.right, out bottomRightPosition);
        }
        
        private static bool CanMoveBottomSide(Board board, Vector2Int position, Vector2Int side, out Vector2Int bottomSidePosition)
        {
            var sidePosition = position + side;
            bottomSidePosition = sidePosition + Vector2Int.down;
            bool canMoveTo = GravityUtils.CanMoveTo(board, bottomSidePosition);
            bool isSideEmpty = !board.MainLayer.ExistsTokenAt(sidePosition);
            bool canMoveToBottomSide = canMoveTo && isSideEmpty;
            return canMoveToBottomSide;
        }
    }
}