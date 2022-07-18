using System.Linq;
using Match3.Core;
using Match3.Core.GameActions.Actions;
using Match3.Core.GameActions.Interactions;
using Match3.Core.Gravity;
using Match3.Core.Levels;
using Match3.Core.TurnSteps;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Match3.Tests.Editor
{
    public static class EditorTestsUtils
    {
        public static GameController GetControllerFromLevelPath(GameContext context, string levelPath)
        {
            var level = AssetDatabase.LoadAssetAtPath<Level>(levelPath);
            var gameController = new GameController(level, context);
            return gameController;
        }
        
        public static GameController GetControllerFromLevelPath(string levelPath)
        {
            var contextAsset = AssetDatabase.LoadAssetAtPath<GameContextAsset>("Assets/Match3/Tests/Editor/TestGameContext.asset");
            var context = contextAsset.GameContext;
            return GetControllerFromLevelPath(context, levelPath);
        }

        public static (IInteraction, IGameAction) GetSwapAction(Vector2Int firstPosition,
            Vector2Int secondPosition)
        {
            var interaction = new SwapInteraction(firstPosition, secondPosition);
            var action = new SwapGameAction();
            return (interaction, action);
        }

        /// <summary>
        /// Get a Vector2Int with less verbose code
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Vector2Int V(int x, int y)
        {
            return new Vector2Int(x, y);
        }
        
        public static Turn MakeMove(GameController gameController, int x1, int y1, int x2, int y2)
        {
            var (interaction, action) = GetSwapAction(V(x1, y1), V(x2, y2));
            var turn = gameController.ExecuteGameAction(interaction, action);
            return turn;
        }

        public static void AssertTokenDestructionAfterMovement(
            GameController gameController,
            int x1,
            int y1,
            int x2,
            int y2,
            BoardLayer layer,
            Vector2Int position,
            bool expectedDestructionState
        )
        {
            var token = layer.GetTokenAt(position);
            var turn = MakeMove(gameController, x1, y1, x2, y2);
            var positionsDestroyed = turn.TurnSteps
                .Where(step => step is TurnStepDestroyTokens)
                .Cast<TurnStepDestroyTokens>()
                .SelectMany(step => step.TokensDestructions)
                .SelectMany(destruction => destruction.DestroyedTokens)
                .Select(positionTokenOrder => positionTokenOrder.Position)
                .ToList();
            bool destroyed = positionsDestroyed.Contains(position);
            Assert.AreEqual(expectedDestructionState, destroyed);
            bool exists = layer.ExistsTokenAt(position);
            bool existsSame = false;
            if (exists)
            {
                var tk = layer.GetTokenAt(position);
                existsSame = tk == token;
            }
            Assert.AreNotEqual(expectedDestructionState, existsSame);
        }
    }
}