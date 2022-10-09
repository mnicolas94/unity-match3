using Match3.Core;
using Match3.Core.GameActions;
using Match3.Core.TurnSteps;
using NUnit.Framework;
using UnityEngine;

namespace Match3.Tests.Editor
{
    public class BoardGameActionsTests
    {
        [TestCase(-1, 0, -1, -1)]
        [TestCase(-1, -1, -1, 0)]
        public void VictoryCondition_Test(int x1, int y1, int x2, int y2)
        {
            // arrange
            var gameController = EditorTestsUtils.GetControllerFromLevelPath("Assets/Match3/Tests/Editor/TestLevels/test-victory-condition.asset");
            
            // act
            var turn = EditorTestsUtils.MakeMove(gameController, x1, y1, x2, y2);
            var turnSteps = turn.TurnSteps.ExecuteTurnStepsNow();
            
            // assert
            bool won = turnSteps.Exists(turnStep => turnStep is TurnStepGameEndVictory);
            bool loose = turnSteps.Exists(turnStep => turnStep is TurnStepGameEndDefeat);
            Assert.IsTrue(won);
            Assert.IsFalse(loose);
        }
        
        [TestCase(1, 0, 1, 1)]
        [TestCase(1, 1, 1, 0)]
        public void DefeatCondition_Test(int x1, int y1, int x2, int y2)
        {
            // arrange
            var gameController = EditorTestsUtils.GetControllerFromLevelPath("Assets/Match3/Tests/Editor/TestLevels/test-victory-condition.asset");
            
            // act
            var turn = EditorTestsUtils.MakeMove(gameController, x1, y1, x2, y2);
            var turnSteps = turn.TurnSteps.ExecuteTurnStepsNow();
            
            // assert
            bool won = turnSteps.Exists(turnStep => turnStep is TurnStepGameEndVictory);
            bool loose = turnSteps.Exists(turnStep => turnStep is TurnStepGameEndDefeat);
            Assert.IsFalse(won);
            Assert.IsTrue(loose);
        }
        
        [Test]
        public void TurnCount_Test()
        {
            // arrange
            var gameController = EditorTestsUtils.GetControllerFromLevelPath("Assets/Match3/Tests/Editor/TestLevels/test-victory-condition.asset");
            
            // false positive assert
            Assert.AreEqual(0, gameController.TurnCount);
            
            // act
            var turn = EditorTestsUtils.MakeMove(gameController, 1, 0, 1, 1);
            turn.TurnSteps.ExecuteTurnStepsNow();
            
            // assert
            Assert.AreEqual(1, gameController.TurnCount);

            // act 2
            turn = EditorTestsUtils.MakeMove(gameController, -1, -1, -1, 0);
            turn.TurnSteps.ExecuteTurnStepsNow();

            // assert 2
            Assert.AreEqual(2, gameController.TurnCount);
        }
        
        [Test]
        public void WhenSolutionsDontExist_ThenShuffle_Test()
        {
            // arrange
            var gameController = EditorTestsUtils.GetControllerFromLevelPath("Assets/Match3/Tests/Editor/TestLevels/test-shuffle.asset");
            
            // act
            var turn = EditorTestsUtils.MakeMove(gameController, 1, 0, 1, 1);
            var turnSteps = turn.TurnSteps.ExecuteTurnStepsNow();
            
            // assert
            bool existsShuffle = turnSteps.Exists(turnStep => turnStep is TurnStepShuffle);
            Assert.IsTrue(existsShuffle);
        }
        
        [Test]
        public void MovingPowerUpIsConsideredASolution_Test()
        {
            // arrange
            var gameController = EditorTestsUtils.GetControllerFromLevelPath(
                "Assets/Match3/Tests/Editor/TestLevels/test-dont-shuffle-with-powerup.asset");
            var powerUpPosition = new Vector2Int(1, 0);
            
            // act
            var existsSolution = gameController.Board.ExistsSolution(gameController.Context);
            var solutions = GamesSimulation.GetAllPossibleMoves(gameController);
            bool allSolutionsContainPosition = solutions.TrueForAll(move =>
            {
                var (posA, posB) = move;
                return posA == powerUpPosition || posB == powerUpPosition;
            });
            
            // assert
            Assert.IsTrue(existsSolution);
            Assert.IsTrue(solutions.Count > 0);
            Assert.IsTrue(allSolutionsContainPosition);
        }
        
        [Test]
        public void WhenOnlySolutionIsSwapPowerUp_ThenNonShuffle_Test()
        {
            // arrange
            var gameController = EditorTestsUtils.GetControllerFromLevelPath(
                "Assets/Match3/Tests/Editor/TestLevels/test-dont-shuffle-with-powerup.asset");
            
            // act
            var turn = gameController.StartGame();
            var turnSteps = turn.TurnSteps.ExecuteTurnStepsNow();
            
            // assert
            bool existsShuffle = turnSteps.Exists(turnStep => turnStep is TurnStepShuffle);
            Assert.IsFalse(existsShuffle);
        }
    }
}
