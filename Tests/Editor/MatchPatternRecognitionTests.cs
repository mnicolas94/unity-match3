using System.Collections.Generic;
using Match3.Core;
using Match3.Core.Matches.Patterns;
using NUnit.Framework;
using UnityEngine;
using ETU = Match3.Tests.Editor.EditorTestsUtils;


namespace Match3.Tests.Editor
{
    public class MatchPatternRecognitionTests
    {
        private void AssertPatternInLevel(
            string levelPath,
            Vector2Int matchPosition,
            MatchPatternRecognizerBase recognizer,
            bool isMet = true
            )
        {
            // arrange
            var gameController = ETU.GetControllerFromLevelPath(levelPath);
            var matches = gameController.Board.GetMatches(gameController.Context);
            var match = matches.Find(m => m.Positions.Contains(matchPosition));

            // act
            bool patternMet = recognizer.MeetsPattern(match);

            // assert
            Assert.AreEqual(isMet, patternMet);
        }
        
        [TestCase(-4, 3)]
        [TestCase(-4, -3)]
        public void RecognizeLine3Match_Test(int x, int y)
        {
            AssertPatternInLevel(
                "Assets/Match3/Tests/Editor/TestLevels/test-matches-patterns.asset",
                ETU.V(x, y),
                MatchPatternTokenCount.GetPatternRecognizer(3));
        }
        
        [TestCase(-3, 3)]
        [TestCase(-4, -4)]
        public void RecognizeLine4Match_Test(int x, int y)
        {
            AssertPatternInLevel(
                "Assets/Match3/Tests/Editor/TestLevels/test-matches-patterns.asset",
                ETU.V(x, y),
                MatchPatternTokenCount.GetPatternRecognizer(4));
        }
        
        [TestCase(-2, 3)]
        public void RecognizeLine5Match_Test(int x, int y)
        {
            AssertPatternInLevel(
                "Assets/Match3/Tests/Editor/TestLevels/test-matches-patterns.asset",
                ETU.V(x, y),
                MatchPatternTokenCount.GetPatternRecognizer(5));
        }
        
        [TestCase(-1, 3)]
        public void RecognizeLine6Match_Test(int x, int y)
        {
            AssertPatternInLevel(
                "Assets/Match3/Tests/Editor/TestLevels/test-matches-patterns.asset",
                ETU.V(x, y),
                MatchPatternTokenCount.GetPatternRecognizer(6));
        }
        
        [TestCase(0, 3)]
        public void RecognizeLine7Match_Test(int x, int y)
        {
            AssertPatternInLevel(
                "Assets/Match3/Tests/Editor/TestLevels/test-matches-patterns.asset",
                ETU.V(x, y),
                MatchPatternTokenCount.GetPatternRecognizer(7));
        }
        
        [TestCase(1, 3)]
        [TestCase(1, 0)]
        public void RecognizeCross5Match_Test(int x, int y)
        {
            AssertPatternInLevel(
                "Assets/Match3/Tests/Editor/TestLevels/test-matches-patterns.asset",
                ETU.V(x, y),
                MatchPatternComposite.GetComposite(new List<MatchPatternRecognizerBase>
                {
                    MatchPatternTokenCount.GetPatternRecognizer(5),
                    ScriptableObject.CreateInstance<MatchPatternCross>()
                }));
        }
        
        [TestCase(2, -1)]
        public void RecognizeCross6Match_Test(int x, int y)
        {
            AssertPatternInLevel(
                "Assets/Match3/Tests/Editor/TestLevels/test-matches-patterns.asset",
                ETU.V(x, y),
                MatchPatternComposite.GetComposite(new List<MatchPatternRecognizerBase>
                {
                    MatchPatternTokenCount.GetPatternRecognizer(6),
                    ScriptableObject.CreateInstance<MatchPatternCross>()
                }));
        }
        
        [TestCase(-4, 3)]
        [TestCase(-4, -3)]
        [TestCase(-3, 3)]
        [TestCase(-4, -4)]
        [TestCase(-2, 3)]
        [TestCase(-1, 3)]
        [TestCase(0, 3)]
        public void RecognizeNotCross_Test(int x, int y)
        {
            AssertPatternInLevel(
                "Assets/Match3/Tests/Editor/TestLevels/test-matches-patterns.asset",
                ETU.V(x, y),
                ScriptableObject.CreateInstance<MatchPatternCross>(),
                false
                );
        }
    }
}