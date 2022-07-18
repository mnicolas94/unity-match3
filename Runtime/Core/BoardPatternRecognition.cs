using System;
using System.Collections.Generic;
using Match3.Core.Gravity;
using Match3.Core.Matches;
using UnityEngine;
using Utils.Extensions;
using Random = UnityEngine.Random;

namespace Match3.Core
{
    public static class BoardPatternRecognition
    {
        public static void FillEmptyTilesWithoutMatches(
            this Board board,
            GameContext context,
            List<TokenData> availableTokens,
            IList<Vector2Int> prohibitedPositions,
            int attemptsLimit = 1000)
        {
            var emptyPositions = board.GetEmptyPositions();
            emptyPositions.RemoveAll(prohibitedPositions.Contains);
            var fixedPositions = board.MainLayer.GetPositions();
            fixedPositions.AddRangeIfNotExists(prohibitedPositions);

            var adjacent = new List<TokenData>();
            var matches = new List<TokenData>();
            var solutions = new List<TokenData>();
            var others = new List<TokenData>(); // not adjacent, nor match, nor solution

            int attempt = 0;
            bool success = false;

            while (!success && attempt < attemptsLimit)
            {
                attempt++;
                bool error = false;

                foreach (var position in emptyPositions)
                {
                    adjacent.Clear();
                    matches.Clear();
                    solutions.Clear();
                    others.Clear();
                    board.GetAdjacentInPosition(context, position, adjacent);
                    board.GetMatchesInPosition(context, position, matches);
                    board.GetSolutionsInPosition(context, position, solutions);
                    adjacent.RemoveAll(token => matches.Contains(token));
                    adjacent.RemoveAll(token => !availableTokens.Contains(token));
                    solutions.RemoveAll(token => matches.Contains(token));
                    solutions.RemoveAll(token => !availableTokens.Contains(token));

                    others.AddRange(availableTokens);
                    others.RemoveAll(token => adjacent.Contains(token));
                    others.RemoveAll(token => matches.Contains(token));
                    others.RemoveAll(token => solutions.Contains(token));

                    var tokensSetsProbabilities = new List<(List<TokenData>, float)>
                    {
                        (others, 0.3f),
                        (solutions, 0.5f),
                        (adjacent, 0.2f),
                    };
                    var tokensProbabilities = new List<(TokenData, float)>();
                    float probSum = 0;
                    foreach (var (tokenSet, setProbability) in tokensSetsProbabilities)
                    {
                        foreach (var token in tokenSet)
                        {
                            float tokenProbability = setProbability / tokenSet.Count;
                            probSum += tokenProbability;
                            tokensProbabilities.Add((token, probSum));
                        }
                    }

                    if (probSum == 0)
                    {
                        // cant make without matches
                        Debug.LogWarning($"Can't fill empty tiles without match in position {position}");
                        error = true;
                        break;
                    }
                    else
                    {
                        float randomValue = Random.value;
                        bool added = false;
                        foreach (var (token, probability) in tokensProbabilities)
                        {
                            float normalizedProbability = probability / probSum;
                            if (randomValue < normalizedProbability)
                            {
                                // poner este token
                                board.MainLayer.AddTokenAt(token, position);
                                added = true;
                                break;
                            }
                        }

                        if (!added)
                        {
                            Debug.Log($"Not added. Random value = {randomValue}");
                        }
                    }
                }

                bool existsSolution = ExistsSolution(board, context);
                bool atLeastOneEmpty = emptyPositions.Count > 0;

                if (!existsSolution && !error && atLeastOneEmpty)
                {
                    bool created = board.CreateSolutionWithoutMatch(context, fixedPositions);
                    if (!created)
                    {
                        error = true;
                    }
                }

                if (error)
                {
                    emptyPositions.ForEach(position =>
                    {
                        if (board.MainLayer.ExistsTokenAt(position))
                        {
                            board.MainLayer.RemoveTokenAt(position);
                        }
                    });
                }
                else
                {
                    success = true;
                }
            }

            if (!success)
            {
                throw new ArgumentException("The level can't be populated without matches and with at least one solution");
            }
        }

        public static bool CreateSolutionWithoutMatch(this Board board, GameContext context, List<Vector2Int> fixedTokens)
        {
            var solutions = new List<TokenData>();
            var matches = new List<TokenData>();
            var positions = board.MainLayer.GetPositions();

            foreach (var position in positions)
            {
                if (fixedTokens.Contains(position)) // do not change this token
                    continue;

                solutions.Clear();
                matches.Clear();
                board.GetSolutionsInPosition(context, position, solutions);
                board.GetMatchesInPosition(context, position, matches);

                solutions.RemoveAll(token => matches.Contains(token));

                if (solutions.Count > 0) // exists solution without match
                {
                    var solution = solutions.GetRandom();
                    board.MainLayer.RemoveTokenAt(position);
                    board.MainLayer.AddTokenAt(solution, position);
                    return true;
                }
            }

            return false;
        }

        public static List<Match> GetMatches(this Board board, GameContext context)
        {
            var matches = new List<Match>();

            var bounds = board.BoardShape.GetBounds();
            var position = new Vector2Int();
            var positions = new List<Vector2Int>();

            void HandleCurrentMatch()
            {
                if (positions.Count >= 3)
                {
                    var match = new Match(positions);
                    matches.Add(match);
                    positions = new List<Vector2Int>();
                }
                else
                {
                    positions.Clear();
                }
            }

            for (int coord = 0; coord <= 1; coord++)
            {
                bool verticalMatchesSearch = coord == 0;
                int iMin = verticalMatchesSearch ? bounds.xMin : bounds.yMin;
                int iMax = verticalMatchesSearch ? bounds.xMax : bounds.yMax;
                int jMin = verticalMatchesSearch ? bounds.yMin : bounds.xMin;
                int jMax = verticalMatchesSearch ? bounds.yMax : bounds.xMax;
                for (int i = iMin; i < iMax; i++)
                {
                    for (int j = jMin; j < jMax; j++)
                    {
                        position.x = verticalMatchesSearch ? i : j;
                        position.y = verticalMatchesSearch ? j : i;
                        int count = positions.Count;
                        var lastToken = count > 0 ? board.MainLayer.GetTokenAt(positions[count - 1]) : null;
                        bool isLastRowOrCol = j == (verticalMatchesSearch ? bounds.yMax - 1 : bounds.xMax - 1);
                        bool hasToken = board.MainLayer.ExistsTokenAt(position);
                        if (hasToken)
                        {
                            var token = board.MainLayer.GetTokenAt(position);
                            bool tokensMatch = context.MatchGroups.DoesTokensMatch(token, lastToken);
                            if (tokensMatch)
                            {
                                positions.Add(position);
                                if (isLastRowOrCol)
                                {
                                    HandleCurrentMatch();
                                }
                            }
                            else
                            {
                                HandleCurrentMatch();
                                if (!isLastRowOrCol)
                                    positions.Add(position);
                            }
                        }
                        else
                        {
                            HandleCurrentMatch();
                        }
                    }
                }
            }

            // merge intersecting matches
            matches = MergeMatches(matches);
            
            return matches;
        }
        
        public static List<Vector2Int> GetContiguousGroup(this Board board, GameContext context, Vector2Int fromPosition)
        {
            var positionsQueue = new List<Vector2Int> {fromPosition};
            var group = new List<Vector2Int>();

            while (positionsQueue.Count > 0)
            {
                var position = positionsQueue[0];
                positionsQueue.RemoveAt(0);
                group.Add(position);

                var token = board.MainLayer.GetTokenAt(position);

                var adjacentPositions = new List<Vector2Int>
                {
                    position + Vector2Int.up,
                    position + Vector2Int.down,
                    position + Vector2Int.left,
                    position + Vector2Int.right
                };
                foreach (var adjacentPosition in adjacentPositions)
                {
                    if (board.MainLayer.ExistsTokenAt(adjacentPosition))
                    {
                        var adjacentToken = board.MainLayer.GetTokenAt(adjacentPosition);
                        if (context.MatchGroups.DoesTokensMatch(token, adjacentToken))
                        {
                            positionsQueue.Add(adjacentPosition);
                        }
                    }
                }
            }

            return group;
        }

        public static bool ExistsSolutionInPosition(this Board board, GameContext context, Vector2Int position, List<TokenData> solutions = null)
        {
            solutions ??= new List<TokenData>();
            solutions.Clear();
            var token = board.MainLayer.GetTokenAt(position);
            board.GetSolutionsInPosition(context, position, solutions);
            bool isSolution = solutions.Contains(token.TokenData);
            return isSolution;
        }

        public static bool ExistsMatchInPosition(this Board board, GameContext context, Vector2Int position, List<TokenData> matches = null)
        {
            matches ??= new List<TokenData>();
            matches.Clear();
            var token = board.MainLayer.GetTokenAt(position);
            board.GetMatchesInPosition(context, position, matches);
            bool isMatch = matches.Contains(token.TokenData);
            return isMatch;
        }

        public static bool ExistsSolution(this Board board, GameContext context)
        {
            var solutions = new List<TokenData>();
            var tokensPositions = board.MainLayer.GetPositions();
            foreach (var position in tokensPositions)
            {
                if (board.ExistsSolutionInPosition(context, position, solutions))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool ExistsMatch(this Board board, GameContext context)
        {
            var matches = new List<TokenData>();
            var tokensPositions = board.MainLayer.GetPositions();
            foreach (var position in tokensPositions)
            {
                if (board.ExistsMatchInPosition(context, position, matches))
                {
                    return true;
                }
            }

            return false;
        }
        
        public static void GetAdjacentInPosition(this Board board, GameContext context, Vector2Int position, List<TokenData> adjacent)
        {
            board.GetTokensWithSelector(context, position, adjacent, GetAdjacentInDirection);
        }

        public static void GetMatchesInPosition(this Board board, GameContext context, Vector2Int position, List<TokenData> matches)
        {
            board.GetTokensWithSelector(context, position, matches, GetMatchInDirection);
            var (horizontalSelected, horizontalToken) = board.GetMatchInDirection(context, position, Vector2Int.right, -1);
            var (verticalSelected, verticalToken) = board.GetMatchInDirection(context, position, Vector2Int.up, -1);

            if (horizontalSelected && !matches.Contains(horizontalToken))
            {
                matches.Add(horizontalToken);
            }

            if (verticalSelected && !matches.Contains(verticalToken))
            {
                matches.Add(verticalToken);
            }
            
            AddMissingTokensFromGroups(context, matches);
        }

        public static void GetSolutionsInPosition(this Board board, GameContext context, Vector2Int position, List<TokenData> solutions)
        {
            board.GetTokensWithSelector(context, position, solutions, GetSolutionInDirection);
            board.GetTokensWithSelector(context, position, solutions, GetSolutionInOrthogonalDirection);
            board.GetTokensWithSelector(context, position, solutions, GetSolutionInOrthogonalNegativeDirection);
            board.GetTokensWithSelector(context, position, solutions, GetSolutionInOrthogonalCenteredDirection);
            
            AddMissingTokensFromGroups(context, solutions);
        }
        
        public static void GetAllSolutionsInDirection(
            this Board board,
            GameContext context,
            Vector2Int position,
            Vector2Int direction,
            List<TokenData> solutions)
        {
            var (exists, token) = board.GetSolutionInDirection(context, position, direction);
            var (existsOrt, tokenOrt) = board.GetSolutionInOrthogonalDirection(context, position, direction);
            var (existsOrtNeg, tokenOrtNeg) = board.GetSolutionInOrthogonalNegativeDirection(context, position, direction);
            var (existsOrtCent, tokenOrtCent) = board.GetSolutionInOrthogonalCenteredDirection(context, position, direction);
            
            if (exists  && !solutions.Contains(token)) solutions.Add(token);
            if (existsOrt  && !solutions.Contains(tokenOrt)) solutions.Add(tokenOrt);
            if (existsOrtNeg  && !solutions.Contains(tokenOrtNeg)) solutions.Add(tokenOrtNeg);
            if (existsOrtCent  && !solutions.Contains(tokenOrtCent)) solutions.Add(tokenOrtCent);
            
            AddMissingTokensFromGroups(context, solutions);
        }

        private static void AddMissingTokensFromGroups(GameContext context, List<TokenData> solutions)
        {
            // add all matches from groups
            for (int i = 0; i < solutions.Count; i++)
            {
                var tokenData = solutions[i];
                var allMatches = context.MatchGroups.GetAllMatchesOfToken(tokenData);
                solutions.AddRangeIfNotExists(allMatches);
            }
        }

        private static List<Match> MergeMatches(List<Match> matches)
        {
            matches = new List<Match>(matches);  // clone to not modify the original
            var newMatches = new List<Match>();

            while (matches.Count > 0)
            {
                var match = matches[0];
                matches.RemoveAt(0);
                bool intersects = true;
                while (intersects)
                {
                    intersects = false;
                    for (int i = 0; i < matches.Count && !intersects; i++)
                    {
                        var otherMatch = matches[i];
                        if (match.Intersects(otherMatch))
                        {
                            match = Match.Merge(match, otherMatch);
                            matches.RemoveAt(i);
                            intersects = true;
                        }
                    }
                }
                
                newMatches.Add(match);
            }

            return newMatches;
        }
        
        private static void GetTokensWithSelector(this Board board, GameContext context,
            Vector2Int position, List<TokenData> tokens,
            Func<Board, GameContext, Vector2Int, Vector2Int, (bool, TokenData)> tokenSelector)
        {
            var (leftSelected, leftToken) = tokenSelector(board, context, position, Vector2Int.left);
            var (rightSelected, rightToken) = tokenSelector(board, context, position, Vector2Int.right);
            var (upSelected, upToken) = tokenSelector(board, context, position, Vector2Int.up);
            var (downSelected, downToken) = tokenSelector(board, context, position, Vector2Int.down);

            if (leftSelected && !tokens.Contains(leftToken))
            {
                tokens.Add(leftToken);
            }

            if (rightSelected && !tokens.Contains(rightToken))
            {
                tokens.Add(rightToken);
            }

            if (upSelected && !tokens.Contains(upToken))
            {
                tokens.Add(upToken);
            }

            if (downSelected && !tokens.Contains(downToken))
            {
                tokens.Add(downToken);
            }
        }

        private static void RemoveTokensWithSelector(this Board board, Vector2Int position, List<TokenData> tokens,
            Func<Board, Vector2Int, Vector2Int, (bool, TokenData)> tokenSelector)
        {
            var (leftSelected, leftToken) = tokenSelector(board, position, Vector2Int.left);
            var (rightSelected, rightToken) = tokenSelector(board, position, Vector2Int.right);
            var (upSelected, upToken) = tokenSelector(board, position, Vector2Int.up);
            var (downSelected, downToken) = tokenSelector(board, position, Vector2Int.down);

            if (leftSelected)
            {
                tokens.Remove(leftToken);
            }

            if (rightSelected)
            {
                tokens.Remove(rightToken);
            }

            if (upSelected)
            {
                tokens.Remove(upToken);
            }

            if (downSelected)
            {
                tokens.Remove(downToken);
            }
        }

        private static (bool, TokenData) GetAdjacentInDirection(
            this Board board, GameContext context,
            Vector2Int position, Vector2Int direction)
        {
            var adjacentPosition = position + direction;
            bool existToken = board.MainLayer.ExistsTokenAt(adjacentPosition);
            if (existToken)
            {
                var adjacentToken = board.MainLayer.GetTokenAt(adjacentPosition);
                return (true, adjacentToken.TokenData);
            }

            return (false, null);
        }

        private static (bool, TokenData) GetMatchInDirection(
            this Board board, GameContext context, 
            Vector2Int position, Vector2Int direction)
        {
            return board.GetMatchInDirection(context, position, direction, 2);
        }

        private static (bool, TokenData) GetMatchInDirection(
            this Board board,
            GameContext context,
            Vector2Int position,
            Vector2Int direction,
            int multiplier)
        {
            var farPosition = position + direction * multiplier;
            var closePosition = position + direction;
            bool existTokens = board.MainLayer.ExistsTokenAt(farPosition) && board.MainLayer.ExistsTokenAt(closePosition);
            if (existTokens)
            {
                var farToken = board.MainLayer.GetTokenAt(farPosition);
                var closeToken = board.MainLayer.GetTokenAt(closePosition);
                if (context.MatchGroups.DoesTokensMatch(farToken, closeToken))
                {
                    return (true, farToken.TokenData);
                }
            }

            return (false, null);
        }

        private static (bool, TokenData) GetSolutionInDirection(
            this Board board, GameContext context, Vector2Int position,
            Vector2Int adjacentDirection, Vector2Int direction, int multiplier)
        {
            var adjacentPosition = position + adjacentDirection;
            bool isValidPosition = board.BoardShape.ExistsPosition(adjacentPosition);
            bool existsToken = board.MainLayer.ExistsTokenAt(adjacentPosition);
            bool canMoveFromPos = GravityUtils.CanMoveFrom(board, position);
            bool canMoveFromAdj = GravityUtils.CanMoveFrom(board, adjacentPosition);
            bool canMove = canMoveFromPos && canMoveFromAdj;
            if (isValidPosition && existsToken && canMove)
            {
                var (existsMatch, token) = board.GetMatchInDirection(context, adjacentPosition, direction, multiplier);
                return (existsMatch, token);
            }

            return (false, null);
        }

        private static (bool, TokenData) GetSolutionInDirection(
            this Board board, GameContext context, Vector2Int position, Vector2Int direction)
        {
            return board.GetSolutionInDirection(context, position, direction, direction, 2);
        }

        private static (bool, TokenData) GetSolutionInOrthogonalDirection(
            this Board board, GameContext context, Vector2Int position, Vector2Int direction)
        {
            var orthogonalDirection = new Vector2Int(direction.y, direction.x);
            return board.GetSolutionInDirection(context, position, direction, orthogonalDirection, 2);
        }

        private static (bool, TokenData) GetSolutionInOrthogonalNegativeDirection(
            this Board board, GameContext context, Vector2Int position, Vector2Int direction)
        {
            var orthogonalNegativeDirection = new Vector2Int(direction.y, direction.x) * -1;
            return board.GetSolutionInDirection(context, position, direction, orthogonalNegativeDirection, 2);
        }

        private static (bool, TokenData) GetSolutionInOrthogonalCenteredDirection(
            this Board board, GameContext context, Vector2Int position, Vector2Int direction)
        {
            var orthogonalDirection = new Vector2Int(direction.y, direction.x);
            return board.GetSolutionInDirection(context, position, direction, orthogonalDirection, -1);
        }
    }
}