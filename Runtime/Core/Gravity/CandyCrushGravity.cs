using System;
using System.Collections.Generic;
using Match3.Core.SerializableTuples;
using Match3.Core.TurnSteps;
using UnityEngine;

namespace Match3.Core.Gravity
{
    using LongReturnType = ValueTuple<List<MovementsList>, Dictionary<Token, (MovementsList, TokenSource)>>;
    
    [Serializable]
    public class CandyCrushGravity : IBoardGravity
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

                            if (CanMoveBottom(board, position, out var bottomPosition))
                            {
                                newPosition = bottomPosition;
                                move = true;
                            }
                            else if (CanMoveBottomRight(board, position, out var botRightPosition))
                            {
                                newPosition = botRightPosition;
                                move = true;
                            }
                            else if (CanMoveBottomLeft(board, position, out var botLeftPosition))
                            {
                                newPosition = botLeftPosition;
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
            bottomSidePosition = position + Vector2Int.down + side;
            bool canMoveTo = GravityUtils.CanMoveTo(board, bottomSidePosition);
            bool canMoveToBottomSide = false;
            if (canMoveTo)
            {
                canMoveToBottomSide = !PositionGetsTokenFromAbove(board, bottomSidePosition);
            }

            return canMoveToBottomSide;
        }

        private static bool PositionGetsTokenFromAbove(Board board, Vector2Int position)
        {
            bool getsTokenFromAbove = false;
            var abovePosition = position + Vector2Int.up;
            bool stopIter = false;
            while (!stopIter && !getsTokenFromAbove)
            {
                bool isValid = board.BoardShape.ExistsPosition(abovePosition);
                bool canMoveFrom = GravityUtils.CanMoveFrom(board, abovePosition);
                bool isEmpty = !board.MainLayer.ExistsTokenAt(abovePosition);
                if (canMoveFrom)
                {
                    getsTokenFromAbove = true;
                }

                stopIter = !canMoveFrom && !(isEmpty && isValid);
                abovePosition += Vector2Int.up;
            }

            return getsTokenFromAbove;
        }
    }
}