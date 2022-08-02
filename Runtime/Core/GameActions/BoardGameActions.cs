using System;
using System.Collections.Generic;
using System.Linq;
using Match3.Core.GameActions.Actions;
using Match3.Core.GameActions.Interactions;
using Match3.Core.GameActions.TokensDamage;
using Match3.Core.Gravity;
using Match3.Core.Matches;
using Match3.Core.SerializableTuples;
using Match3.Core.TokensEvents.Outputs;
using Match3.Core.TurnSteps;
using Match3.Core.TurnSteps.TokensDestructionSource;
using UnityEngine;
using Utils.Extensions;

namespace Match3.Core.GameActions
{
    /// <summary>
    /// Method suffixes:
    /// - Gts_...: returns an iterable of turn steps
    /// - T_...: transform the board
    /// </summary>
    public static class BoardGameActions
    {
        public static Turn ExecuteAction(
            GameContext context,
            Board board,
            IInteraction interaction,
            IGameAction action,
            Func<Vector2Int, TokenSource, TokenData> requestTokenFunction)
        {
            if (!action.IsInteractionValid(board, interaction))
                return Turn.EmptyTurn;
                
            var execution = action.Execute(context, board, interaction);
            var actionTurnSteps = execution.TurnSteps;
            var commonTurnSteps = Gts_CommonStartWithGravity(context, board, requestTokenFunction);
            var turnSteps = actionTurnSteps.Concat(commonTurnSteps);
            var turn = new Turn(execution.CountAsTurn, turnSteps);
            return turn;
        }
        
        private static IEnumerable<TurnStep> Gts_Common(
            GameContext context,
            Board board,
            Func<Vector2Int, TokenSource, TokenData> requestTokenFunction)
        {
            bool keepLooping = true;
            while (keepLooping)
            {
                bool existsMatch = board.ExistsMatch(context);
                while (existsMatch)
                {
                    var matches = board.GetMatches(context);
                    var matchTurnSteps = Gts_Matches(context, board, matches);
                    var gravityTurnSteps = Gts_ApplyGravity(context, board, requestTokenFunction);

                    var turnSteps = matchTurnSteps.Concat(gravityTurnSteps);
                    foreach (var turnStep in turnSteps)
                    {
                        yield return turnStep;
                    }

                    existsMatch = board.ExistsMatch(context);
                }
                bool existsSolution = board.ExistsSolution(context);
                if (!existsSolution)
                {
                    // shuffle
                    yield return Gts_ShuffleTokens(context, board);
                }

                keepLooping = !existsSolution;
            }
        }
        
        private static IEnumerable<TurnStep> Gts_CommonStartWithGravity(
            GameContext context,
            Board board,
            Func<Vector2Int, TokenSource, TokenData> requestTokenFunction)
        {
            var gravityTurnSteps = Gts_ApplyGravity(context, board, requestTokenFunction);
            var commonTurnSteps = Gts_Common(context, board, requestTokenFunction);
            var turnSteps = gravityTurnSteps.Concat(commonTurnSteps);
            foreach (var turnStep in turnSteps)
            {
                yield return turnStep;
            }
        }

        internal static IEnumerable<TurnStep> Gts_Matches(
            GameContext context,
            Board board,
            List<Match> matches,
            List<TokenEventOutput> swapOutputs = null
        )
        {
            Vector2Int GetRandom(IList<Vector2Int> positions)
            {
                return positions.GetRandom();
            }
            return Gts_Matches(context, board, matches, GetRandom, swapOutputs);
        }

        internal static IEnumerable<TurnStep> Gts_Matches(
            GameContext context,
            Board board,
            List<Match> matches,
            Func<IList<Vector2Int>, Vector2Int> getSpawnPositionFunction,
            List<TokenEventOutput> swapOutputs = null
            )
        {
            var tokensToTransform = new List<(List<PositionToken>, Match, PatternToToken)>();
            foreach (var match in matches)
            {
                var (patternToToken, existsPattern) = context.MatchGroups.FirstPatternMet(board, match);
                if (!existsPattern)
                    continue;
                var matched = new List<PositionToken>();
                foreach (var position in match.Positions)
                {
                    var token = board.MainLayer.GetTokenAt(position);
                    bool hasTokenAbove = board.TopLayers.ExistsAnyTokenAt(position);
                    if (!hasTokenAbove)
                        matched.Add(new PositionToken(position, token));
                }

                tokensToTransform.Add((matched, match, patternToToken));
            }
            
            var positionsInMatches = matches.SelectMany(match => match.Positions).ToList();
            
            // send below and adjacent matched events
            swapOutputs ??= new List<TokenEventOutput>();
            var outputs = new List<TokenEventOutput>();
            SendEventUtils.SendBelowMatchedEvent(context, board, positionsInMatches, outputs);
            SendEventUtils.SendAdjacentMatchEvent(context, board, positionsInMatches, outputs);
            outputs.AddRange(swapOutputs);
            var matchesTurnSteps = Gts_HandleEventOutputs(context, board, outputs).ExecuteTurnStepsNow();
            
            // damage matched tokens
            var destroyedTokens = new List<TokensDamaged>();
            foreach (var match in matches)
            {
                var positionsToDamage = match.Positions.Select(position => new PositionToAttackOrder(position, 0));
                var destroyed = T_AttackPositions(context, board, positionsToDamage);
                var randomPosition = match.Positions.GetRandom();
                var destructionSource = new DestructionSourceMatch(match);
                var destruction = new TokensDamaged(destructionSource, randomPosition, destroyed);
                destroyedTokens.Add(destruction);
            }
            var attackPositionsTurnSteps = Gts_HandleTokensDestruction(context, board, destroyedTokens).ExecuteTurnStepsNow();
            
            // spawn new tokens due to transformations
            List<Transformation> transformations = new List<Transformation>();
            foreach (var (matchedTokens, match, (pattern, tokenProvider)) in tokensToTransform)
            {
                var emptyPositions = matchedTokens.ConvertAll(posToken => posToken.Position)
                    .FindAll(board.IsPositionEmpty);
                bool anyPositionEmpty = emptyPositions.Count > 0;
                if (!anyPositionEmpty)
                    continue;
                var spawnPosition = getSpawnPositionFunction(emptyPositions);
                var tokenToSpawn = tokenProvider.GetToken();
                var newToken = board.MainLayer.AddTokenAt(tokenToSpawn, spawnPosition);
                var transformation = new Transformation
                (
                    new MatchTransformationSource(match, pattern), 
                    matchedTokens,
                    new PositionToken(spawnPosition, newToken)
                );
                transformations.Add(transformation);
            }

            if (transformations.Count > 0)
            {
                var turnStepTransformations = new TurnStepTransformations(transformations);
                yield return turnStepTransformations;
            }
            var turnSteps = attackPositionsTurnSteps.Concat(matchesTurnSteps);
            foreach (var turnStep in turnSteps)
            {
                yield return turnStep;
            }
        }
        
        internal static IEnumerable<TurnStep> Gts_ApplyGravity(
            GameContext context,
            Board board,
            Func<Vector2Int, TokenSource, TokenData> requestTokenFunction)
        {
            // apply gravity and fill with new tokens
            bool keepLooping = true;

            while (keepLooping)
            {
                var (turnMovements, fakeMovements) = context.Gravity.GetTurnStepMovements(board);
                keepLooping = turnMovements.Count > 0;

                // replace fake token with new ones
                foreach (var fakeToken in fakeMovements.Keys)
                {
                    var (fakeMoves, tokenSource) = fakeMovements[fakeToken];
                    var lastPosition = fakeMoves[fakeMoves.Count - 1].ToPosition;
                    var realTokenData = requestTokenFunction(lastPosition, tokenSource);
                    var realToken = new Token(realTokenData);
                    foreach (var fakeMovement in fakeMoves)
                    {
                        fakeMovement.Token = realToken;
                    }

                    var position = board.MainLayer.GetPositionOfToken(fakeToken);
                    board.MainLayer.RemoveToken(fakeToken);
                    board.MainLayer.AddTokenAt(realToken, position);
                }

                if (turnMovements.Count == 0)
                    yield break;
                var turnStepMovements = new TurnStepMovements(turnMovements);
                yield return turnStepMovements;

                var outputs = GetOutputsFromReachBottom(context, board, turnMovements);
                var turnSteps = Gts_HandleEventOutputs(context, board, outputs);
                foreach (var turnStep in turnSteps)
                {
                    yield return turnStep;
                }
            }
        }
        
        internal static TurnStep Gts_ShuffleTokens(GameContext context, Board board)
        {
            var shuffleMovements = new List<TokenMovement>();
            bool existsSolution = false;
            while (!existsSolution)
            {
                shuffleMovements = board.Shuffle();
                existsSolution = board.ExistsSolution(context);
            }

            var turnStepShuffle = new TurnStepShuffle(shuffleMovements);
            return turnStepShuffle;
        }
        
        internal static IEnumerable<TurnStep> Gts_HandleEventOutputs(
            GameContext context,
            Board board,
            List<TokenEventOutput> outputs)
        {
            while (outputs.Count > 0)
            {
                var destroyedTokens = T_HandleEventOutputs(context, board, outputs);
                if (destroyedTokens.Count == 0)
                    break;

                var turnStepDestruction = new TurnStepDamageTokens(destroyedTokens);
                yield return turnStepDestruction;

                outputs.Clear();
                GetOutputsFromTokensDestruction(context, board, destroyedTokens, outputs);
            }
        }
        
        internal static IEnumerable<TurnStep> Gts_HandleTokensDestruction(
            GameContext context,
            Board board,
            List<TokensDamaged> destroyedTokens)
        {
            while (destroyedTokens.Count > 0)
            {
                var turnStepDestruction = new TurnStepDamageTokens(destroyedTokens);
                yield return turnStepDestruction;

                var outputs = new List<TokenEventOutput>();
                GetOutputsFromTokensDestruction(context, board, destroyedTokens, outputs);
                if (outputs.Count <= 0) 
                    break;
            
                destroyedTokens = T_HandleEventOutputs(context, board, outputs);
            }
        }

        internal static List<TokensDamaged> T_HandleEventOutputs(
            GameContext context,
            Board board,
            List<TokenEventOutput> outputs)
        {
            var destroyedTokens = new List<TokensDamaged>();
            
            // DamagePositionsEventOutput
            var damageOutputs = outputs.FindAll(output => output is DamagePositionsEventOutput);
            foreach (var output in damageOutputs)
            {
                var damageOutput = (DamagePositionsEventOutput) output;
                var destroyed = T_AttackPositions(context, board, damageOutput.PositionsToDamage);
                var destruction = new TokensDamaged(damageOutput.DamageSource, damageOutput.SourcePosition, destroyed);
                destroyedTokens.Add(destruction);
            }

            // DestroyPositionsEventOutput
            var destroyOutputs = outputs.FindAll(output => output is DestroyTokensEventOutput);
            foreach (var output in destroyOutputs)
            {
                var destroyOutput = (DestroyTokensEventOutput) output;
                destroyedTokens.Add(destroyOutput.TokensToDestroy);

                // destroy from board
                foreach (var (position, token, destroyOrder, damageInfo) in destroyOutput.TokensToDestroy.DestroyedTokens)
                {
                    var layer = board.TryGetLayerOfTokenAtPosition(token, position, out bool existsLayer);
                    layer.RemoveTokenAt(position);
                }
            }

            return destroyedTokens;
        }

        internal static List<PositionTokenDamageOrder> T_AttackPositions(
            GameContext context,
            Board board,
            IEnumerable<PositionToAttackOrder> positionsToAttack)
        {
            var destroyedTokens = new List<PositionTokenDamageOrder>();

            bool DestructiblePredicate(Token token)
            {
                return !token.TokenData.IsIndestructible;
            }
            
            foreach (var (position, damageOrder) in positionsToAttack)
            {
                bool exists = board.ExistsTokenAnyLayerAtWhere(position, DestructiblePredicate);
                if (!exists)
                    continue;
                var (token, layer) = board.GetTopTokenAtWhere(position, DestructiblePredicate);
                int damage = 1;  // TODO get from damage manager at GameContext
                var damageDone = token.ApplyDamage(damage, position);
                var damageInfo = new DamageInfo(damageDone);
                var positionTokenDamageOrder = new PositionTokenDamageOrder(position, token, damageOrder, damageInfo);
                destroyedTokens.Add(positionTokenDamageOrder);
                var destroyIt = token.HealthPoints <= 0;
                if (destroyIt)
                    layer.RemoveTokenAt(position);
            }

            return destroyedTokens;
        }

        internal static void GetOutputsFromTokensDestruction(
            GameContext context,
            Board board,
            List<TokensDamaged> destroyedTokens,
            List<TokenEventOutput> outputs)
        {
            var positionTokens = destroyedTokens.GetAllPositionsTokens().ToList();
            var destroyedPositions = positionTokens.ConvertAll(posToken => posToken.Position);

            SendEventUtils.SendDestroyedEvent(context, board, positionTokens, outputs);
            SendEventUtils.SendAboveDestroyedEvent(context, board, destroyedPositions, outputs);
            SendEventUtils.SendAdjacentDestroyedEvent(context, board, destroyedPositions, outputs);
        }
        
        private static List<TokenEventOutput> GetOutputsFromReachBottom(
            GameContext context,
            Board board,
            List<MovementsList> turnMovements)
        {
            var bottomReachedTokens = new List<PositionToken>();
            foreach (var movements in turnMovements)
            {
                foreach (var (token, fromPosition, toPosition) in movements)
                {
                    bool isLimit = board.BoardShape.IsLimitInDirection(toPosition, Vector2Int.down);
                    if (isLimit)
                    {
                        bottomReachedTokens.Add(new PositionToken(toPosition, token));
                    }
                }
            }
            
            var outputs = new List<TokenEventOutput>();
            if (bottomReachedTokens.Count > 0)
            {
                SendEventUtils.SendReachBottomEvent(context, board, bottomReachedTokens, outputs);
            }

            return outputs;
        }
        
        public static List<TurnStep> ExecuteTurnStepsNow(this IEnumerable<TurnStep> turnSteps)
        {
            var turnStepsList = new List<TurnStep>();
            foreach (var turnStep in turnSteps)
            {
                turnStepsList.Add(turnStep);
            }
            return turnStepsList;
        }

    }
}