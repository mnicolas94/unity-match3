using Match3.Core.GameActions;
using Match3.Core.TurnSteps;
using NUnit.Framework;

namespace Match3.Tests.Editor
{
    public class TokensEventsTests
    {
        [TestCase(2, -1, 3, -2, 3, -3)]
        [TestCase(2, -1, 4, 1, 4, 0)]
        [TestCase(2, -1, 3, 0, 4, 0)]
        public void TestAdjacentMatchEvent_Test(int x, int y, int mx1, int my1, int mx2, int my2)
        {
            // arrange
            string levelPath = "Assets/Match3/Tests/Editor/TestLevels/test-several-use-cases.asset";
            var gameController = EditorTestsUtils.GetControllerFromLevelPath(levelPath);

            // act and assert
            EditorTestsUtils.AssertTokenDestructionAfterMovement(
                gameController,
                mx1, my1, mx2, my2,
                gameController.Board.MainLayer,
                EditorTestsUtils.V(x, y),
                true
            );
        }
        
        [TestCase(1, -2, 3, -2, 3, -3)]
        [TestCase(-4, 0, -2, 0, -2, 1)]
        [TestCase(-4, 0, -2, 0, -1, 0)]
        public void TestBelowMatchEvent_Test(int x, int y, int mx1, int my1, int mx2, int my2)
        {
            // arrange
            string levelPath = "Assets/Match3/Tests/Editor/TestLevels/test-several-use-cases.asset";
            var gameController = EditorTestsUtils.GetControllerFromLevelPath(levelPath);

            // act and assert
            EditorTestsUtils.AssertTokenDestructionAfterMovement(
                gameController,
                mx1, my1, mx2, my2,
                gameController.Board.TopLayers[0],
                EditorTestsUtils.V(x, y),
                true
            );
        }
        
        [TestCase(-2, 2, -2, 0, -1, 0)]
        [TestCase(-2, 3, 0, 3, -1, 3)]
        [TestCase(-1, 3, 0, 3, -1, 3)]
        public void TestAboveDestroyedEvent_Test(int x, int y, int mx1, int my1, int mx2, int my2)
        {
            // arrange
            string levelPath = "Assets/Match3/Tests/Editor/TestLevels/test-several-use-cases.asset";
            var gameController = EditorTestsUtils.GetControllerFromLevelPath(levelPath);

            // act and assert
            EditorTestsUtils.AssertTokenDestructionAfterMovement(
                gameController,
                mx1, my1, mx2, my2,
                gameController.Board.BottomLayers[0],
                EditorTestsUtils.V(x, y),
                true
            );
        }
        
        [TestCase(-3, -3, -2, -4, -1, -4)]
        [TestCase(-4, -3, -2, -4, -1, -4)]
        public void TestReachBottomEvent_Test(int x, int y, int mx1, int my1, int mx2, int my2)
        {
            // arrange
            string levelPath = "Assets/Match3/Tests/Editor/TestLevels/test-several-use-cases.asset";
            var gameController = EditorTestsUtils.GetControllerFromLevelPath(levelPath);
            var layer = gameController.Board.MainLayer;
            var position = EditorTestsUtils.V(x, y);
            var token = layer.GetTokenAt(position);

            // act
            var turn = EditorTestsUtils.MakeMove(gameController, mx1, my1, mx2, my2);
            turn.TurnSteps.ExecuteTurnStepsNow();
            
            // assert
            bool exists = layer.ExistsTokenAt(position);
            bool isStillInBoard = false;
            if (exists)
            {
                var tk = layer.GetTokenAt(position);
                isStillInBoard = tk == token;
            }
            Assert.IsFalse(isStillInBoard);
        }
        
        [TestCase(-2, -2, -2, -3)]
        [TestCase(-2, -2, -2, -1)]
        [TestCase(2, -4, 2, -3)]
        [TestCase(2, -4, 1, -4)]
        [TestCase(2, -4, 3, -4)]
        public void TestSwappedEvent_Test(int mx1, int my1, int mx2, int my2)
        {
            // arrange
            string levelPath = "Assets/Match3/Tests/Editor/TestLevels/test-several-use-cases.asset";
            var gameController = EditorTestsUtils.GetControllerFromLevelPath(levelPath);

            // act
            var turn = EditorTestsUtils.MakeMove(gameController, mx1, my1, mx2, my2);
            var turnSteps = turn.TurnSteps.ExecuteTurnStepsNow();
            
            // assert
            bool powerUpActivated = turnSteps.Exists(step => step is TurnStepDestroyTokens);
            Assert.IsTrue(powerUpActivated);
        }
    }
}