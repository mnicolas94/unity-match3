using System.Linq;
using Match3.Core.TurnSteps;
using NUnit.Framework;

namespace Match3.Tests.Editor
{
    public class TokenDataFlagsTests
    {
        [Test]
        public void WhenCanMoveIsFalse_TokenDoesNotFall_Test()
        {
            // arrange
            string levelPath = "Assets/Match3/Tests/Editor/TestLevels/test-several-use-cases.asset";
            var gameController = EditorTestsUtils.GetControllerFromLevelPath(levelPath);
            var blockPosition = EditorTestsUtils.V(2, -1);
            var token = gameController.Board.MainLayer.GetTokenAt(blockPosition);

            // act
            var turn = EditorTestsUtils.MakeMove(gameController, 2, -2, 2, -3);
            bool moved = turn.TurnSteps
                .Where(step => step is TurnStepMovements)
                .Cast<TurnStepMovements>()
                .SelectMany(step => step.TokensPaths)
                .SelectMany(path => path)
                .ToList()
                .Exists(movement => movement.Token == token);
            
            // assert
            Assert.IsFalse(moved);
            var tk = gameController.Board.MainLayer.GetTokenAt(blockPosition);
            Assert.AreSame(token, tk);
        }
        
        [TestCase(-2, 3, -2, 2, -1, 2)]
        [TestCase(-4, 3, -2, 2, -1, 2)]
        [TestCase(3, -1, 2, -2, 2, -3)]
        [TestCase(3, -2, 2, -2, 2, -3)]
        [TestCase(3, 3, 2, -2, 2, -3)]
        public void WhenCanMoveIsTrue_TokenDoFall_Test(int posX, int posY, int mx1, int my1, int mx2, int my2)
        {
            // arrange
            string levelPath = "Assets/Match3/Tests/Editor/TestLevels/test-several-use-cases.asset";
            var gameController = EditorTestsUtils.GetControllerFromLevelPath(levelPath);
            var tokenPosition = EditorTestsUtils.V(posX, posY);
            var token = gameController.Board.MainLayer.GetTokenAt(tokenPosition);

            // act
            var turn = EditorTestsUtils.MakeMove(gameController, mx1, my1, mx2, my2);
            bool moved = turn.TurnSteps
                .Where(step => step is TurnStepMovements)
                .Cast<TurnStepMovements>()
                .SelectMany(step => step.TokensPaths)
                .SelectMany(path => path)
                .ToList()
                .Exists(movement => movement.Token == token);
            
            // assert
            Assert.IsTrue(moved);
            var tk = gameController.Board.MainLayer.GetTokenAt(tokenPosition);
            Assert.AreNotSame(token, tk);
        }

        [Test]
        public void WhenCanMatchWithItselfIsFalse_CantMatch_Test()
        {
            // arrange
            string levelPath = "Assets/Match3/Tests/Editor/TestLevels/test-several-use-cases.asset";
            var gameController = EditorTestsUtils.GetControllerFromLevelPath(levelPath);

            // act
            var turn = EditorTestsUtils.MakeMove(gameController, -1, -3, -2, -3);
            bool matched = turn.TurnSteps.ToList().Exists(step => step is TurnStepDamageTokens);
            
            // assert
            Assert.IsFalse(matched);
        }
        
        [TestCase(-2, 2, -1, 2)]
        [TestCase(2, -2, 2, -3)]
        public void WhenCanMatchWithItselfIsTrue_CanMatch_Test(int x1, int y1, int x2, int y2)
        {
            // arrange
            string levelPath = "Assets/Match3/Tests/Editor/TestLevels/test-several-use-cases.asset";
            var gameController = EditorTestsUtils.GetControllerFromLevelPath(levelPath);

            // act
            var turn = EditorTestsUtils.MakeMove(gameController, x1, y1, x2, y2);
            bool matched = turn.TurnSteps.ToList().Exists(step => step is TurnStepDamageTokens);
            
            // assert
            Assert.IsTrue(matched);
        }

        [TestCase(-1, -3, false)]
        [TestCase(-3, -3, false)]
        [TestCase(-4, -3, false)]
        [TestCase(1, -3, true)]
        [TestCase(0, -3, true)]
        public void WhenTokenIsIndestructible_CannotBeDestroyed(int x, int y, bool expectedPositionIsDestroyed)
        {
            // arrange
            string levelPath = "Assets/Match3/Tests/Editor/TestLevels/test-several-use-cases.asset";
            var gameController = EditorTestsUtils.GetControllerFromLevelPath(levelPath);

            // act
            EditorTestsUtils.AssertTokenDestructionAfterMovement(
                gameController,
                -2, -2, -2, -3,
                gameController.Board.MainLayer,
                EditorTestsUtils.V(x, y),
                expectedPositionIsDestroyed
            );
        }
    }
}