using System.Linq;
using Match3.Core.GameActions;
using Match3.Core.TurnSteps;
using NUnit.Framework;

namespace Match3.Tests.Editor
{
    public class MatchTransformationsTests
    {
        [TestCase(-2, 0, -2, 1)]
        [TestCase(-2, 1, -2, 0)]
        [TestCase(-2, 0, -1, 0)]
        [TestCase(-1, 0, -2, 0)]
        [TestCase(0, 0, 1, 0)]
        [TestCase(2, 2, 2, 3)]
        [TestCase(3, 2, 4, 2)]
        [TestCase(3, 2, 3, 3)]
        public void WhenMatchingMoreThan4Tokens_ATransformationOccurs_Test(int mx1, int my1, int mx2, int my2)
        {
            // arrange
            var levelPath = "Assets/Match3/Tests/Editor/TestLevels/test-several-use-cases.asset";
            var gameController = EditorTestsUtils.GetControllerFromLevelPath(levelPath);
            
            // act
            var turn = EditorTestsUtils.MakeMove(gameController, mx1, my1, mx2, my2);
            var turnSteps = turn.TurnSteps.ExecuteTurnStepsNow();
            
            // assert
            bool existsTransformation = turnSteps.Exists(turnStep => turnStep is TurnStepTransformations);
            Assert.IsTrue(existsTransformation);
        }
        
        [TestCase(3, 2, 3, 1)]
        [TestCase(3, 2, 3, 3)]
        public void WhenGeneratePowerUpWhileSwappingAnotherPower_TheGeneratedOneIsNotDestroyed_Test(int mx1, int my1, int mx2, int my2)
        {
            // arrange
            var levelPath = "Assets/Match3/Tests/Editor/TestLevels/test-several-use-cases.asset";
            var gameController = EditorTestsUtils.GetControllerFromLevelPath(levelPath);
            
            // act
            var turn = EditorTestsUtils.MakeMove(gameController, mx1, my1, mx2, my2);
            var turnSteps = turn.TurnSteps.ExecuteTurnStepsNow();
            
            // assert
            var spawnedTokens = turnSteps.Where(turnStep => turnStep is TurnStepTransformations)
                .Cast<TurnStepTransformations>()
                .SelectMany(step => step.Transformations)
                .Select(transformation => transformation.SpawnedToken.token);

            foreach (var token in spawnedTokens)
            {
                bool exists = gameController.Board.ExistsTokenInAnyLayer(token);
                Assert.IsTrue(exists);
            }
        }
        
        [TestCase(-4, 0, -2, 0, -2, 1)]
        [TestCase(-4, 0, -2, 0, -1, 0)]
        public void WhenGeneratingPowerUpAndTokenHaveAboveToken_ThatTokenIsNotAddedToTransformation_Test(
            int x, int y, int mx1, int my1, int mx2, int my2)
        {
            // arrange
            var levelPath = "Assets/Match3/Tests/Editor/TestLevels/test-several-use-cases.asset";
            var gameController = EditorTestsUtils.GetControllerFromLevelPath(levelPath);
            
            // act
            var turn = EditorTestsUtils.MakeMove(gameController, mx1, my1, mx2, my2);
            var turnSteps = turn.TurnSteps.ExecuteTurnStepsNow();
            
            // assert
            bool existsPosition = turnSteps.Where(turnStep => turnStep is TurnStepTransformations)
                .Cast<TurnStepTransformations>()
                .SelectMany(step => step.Transformations)
                .SelectMany(transformation => transformation.TransformedTokens)
                .Select(positionToken => positionToken.position)
                .ToList()
                .Contains(EditorTestsUtils.V(x, y));

            Assert.IsFalse(existsPosition);
        }
        
        [TestCase(-2, 0, -2, 1)]
        [TestCase(-2, 1, -2, 0)]
        [TestCase(-2, 0, -1, 0)]
        [TestCase(-1, 0, -2, 0)]
        [TestCase(0, 0, 1, 0)]
        [TestCase(2, 2, 2, 3)]
        [TestCase(3, 2, 4, 2)]
        [TestCase(3, 2, 3, 3)]
        public void WhenTransformationOccurs_ThePowerUpIsGeneratedInTheSwapPosition_Test(int mx1, int my1, int mx2, int my2)
        {
            // arrange
            var levelPath = "Assets/Match3/Tests/Editor/TestLevels/test-several-use-cases.asset";

            int tries = 20;
            for (int i = 0; i < tries; i++)  // try several times to avoid false-positives due to randomness
            {
                var gameController = EditorTestsUtils.GetControllerFromLevelPath(levelPath);
            
                // act
                var turn = EditorTestsUtils.MakeMove(gameController, mx1, my1, mx2, my2);
                var turnSteps = turn.TurnSteps.ExecuteTurnStepsNow();
            
                // assert
                var generatedPosition = turnSteps.Where(turnStep => turnStep is TurnStepTransformations)
                    .Cast<TurnStepTransformations>()
                    .SelectMany(step => step.Transformations)
                    .Select(transformation => transformation.SpawnedToken.position)
                    .First();

                var v1 = EditorTestsUtils.V(mx1, my1);
                var v2 = EditorTestsUtils.V(mx2, my2);
                bool isInSwapPosition = generatedPosition == v1 || generatedPosition == v2;

                Assert.IsTrue(isInSwapPosition);
            }
        }
    }
}