using System.Collections.Generic;
using Match3.Core;
using Match3.Core.Levels;
using Match3.Core.Matches;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Match3.Tests.Editor
{
    public class BoardRecognitionPatternTests
    {
        private const int Board3X3 = 0;
        private const int Board5X5 = 1;

        private TokenData tokenRed;
        private TokenData tokenBlue;
        private TokenData tokenBlueSmall;
        private TokenData tokenGreen;
        private TokenData tokenYellow;
        
        private List<Board> boards;
        private List<List<Vector2Int>> solutions;
        private List<List<Vector2Int>> matches;
        private List<List<Match>> matchesObjects;
        
        private List<List<(Vector2Int, List<TokenData>)>> adjacents;
        private List<List<(Vector2Int, List<TokenData>)>> potentialSolutions;
        private List<List<(Vector2Int, List<TokenData>)>> potentialMatches;

        private GameContext _context;
        
        [SetUp]
        public void Setup()
        {
            var contextAsset = AssetDatabase.LoadAssetAtPath<GameContextAsset>("Assets/Match3/Tests/Editor/TestGameContext.asset");
            _context = contextAsset.GameContextCopy;
            
            var level3X3 = AssetDatabase.LoadAssetAtPath<Level>("Assets/Match3/Tests/Editor/TestLevels/test3x3.asset");
            var level5X5 = AssetDatabase.LoadAssetAtPath<Level>("Assets/Match3/Tests/Editor/TestLevels/test5x5.asset");

            tokenRed = level3X3.TokensCreationData.Tokens[0];
            tokenBlue = level3X3.TokensCreationData.Tokens[1];
            tokenGreen = level3X3.TokensCreationData.Tokens[2];
            tokenYellow = level3X3.TokensCreationData.Tokens[3];
            tokenBlueSmall = level3X3.TokensCreationData.Tokens[4];
            
            boards = new List<Board>
            {
                Board.PopulateLevel(level3X3, _context),
                Board.PopulateLevel(level5X5, _context),
            };
            
            solutions = new List<List<Vector2Int>>
            {
                new List<Vector2Int>  // solutions in 3x3 board
                {
                    new Vector2Int(2, 0),
                },
                new List<Vector2Int>()  // solutions in 5x5 board
            };
            
            matches = new List<List<Vector2Int>>
            {
                new List<Vector2Int>(),  // matches in 3x3 board
                new List<Vector2Int>    // matches in 5x5 board
                {
                    new Vector2Int(4, 1),
                    new Vector2Int(4, 2),
                    new Vector2Int(4, 3)
                }
            };
            
            matchesObjects = new List<List<Match>>
            {
                new List<Match>(),  // matches in 3x3 board
                new List<Match>    // matches in 5x5 board
                {
                    new Match(new List<Vector2Int>
                    {
                        new Vector2Int(4, 1),
                        new Vector2Int(4, 2),
                        new Vector2Int(4, 3)
                    })
                }
            };
            
            adjacents = new List<List<(Vector2Int, List<TokenData>)>>
            {
                new List<(Vector2Int, List<TokenData>)>  // some adjacents in 3x3 board
                {
                    (new Vector2Int(0, 1), new List<TokenData> {tokenGreen, tokenBlue, tokenRed}),
                    (new Vector2Int(2, 2), new List<TokenData> {tokenGreen, tokenRed}),
                    (new Vector2Int(1, 1), new List<TokenData> {tokenGreen, tokenBlue, tokenYellow, tokenRed}),
                },
                new List<(Vector2Int, List<TokenData>)>  // some adjacents in 5x5 board
                {
                    (new Vector2Int(0, 1), new List<TokenData> {tokenYellow, tokenRed}),
                    (new Vector2Int(1, 2), new List<TokenData> {tokenYellow}),
                    (new Vector2Int(2, 4), new List<TokenData> {tokenGreen, tokenBlue, tokenRed}),
                }
            };
            
            potentialSolutions = new List<List<(Vector2Int, List<TokenData>)>>
            {
                new List<(Vector2Int, List<TokenData>)>  // some solutions in 3x3 board
                {
                    (new Vector2Int(2, 2), new List<TokenData> {tokenBlue, tokenBlueSmall}),
                    (new Vector2Int(2, 0), new List<TokenData> {tokenBlue, tokenBlueSmall})
                },
                new List<(Vector2Int, List<TokenData>)>  // some solutions in 5x5 board
                {
                    (new Vector2Int(0, 0), new List<TokenData> {tokenYellow}),
                    (new Vector2Int(1, 4), new List<TokenData> {tokenYellow, tokenGreen}),
                    (new Vector2Int(2, 1), new List<TokenData> {})
                }
            };
            
            potentialMatches = new List<List<(Vector2Int, List<TokenData>)>>
            {
                new List<(Vector2Int, List<TokenData>)>  // some matches in 3x3 board
                {
                    (new Vector2Int(0, 0), new List<TokenData> {}),
                    (new Vector2Int(2, 1), new List<TokenData> {tokenBlue, tokenBlueSmall})
                },
                new List<(Vector2Int, List<TokenData>)>  // some matches in 5x5 board
                {
                    (new Vector2Int(0, 1), new List<TokenData> {tokenYellow}),
                    (new Vector2Int(0, 4), new List<TokenData> {tokenYellow}),
                    (new Vector2Int(2, 4), new List<TokenData> {tokenGreen}),
                    (new Vector2Int(4, 4), new List<TokenData> {tokenRed})
                }
            };
        }

        [TestCase(Board3X3)]
        [TestCase(Board5X5)]
        public void ExistsSolutionInPosition_Test(int boardIndex)
        {
            // arrange
            var board = boards[boardIndex];
            var sols = solutions[boardIndex];
            var positions = board.MainLayer.GetPositions();
            
            foreach (var solution in sols)
            {
                // act
                var result = board.ExistsSolutionInPosition(_context, solution);
                // assert
                Assert.IsTrue(result);
                positions.Remove(solution);
            }
            
            foreach (var position in positions)
            {
                // act
                var result = board.ExistsSolutionInPosition(_context, position);
                // assert
                Assert.IsFalse(result);
            }
        }
        
        [TestCase(Board3X3)]
        [TestCase(Board5X5)]
        public void ExistsSolution_Test(int boardIndex)
        {
            // arrange
            var board = boards[boardIndex];
            var sols = solutions[boardIndex];
            bool expected = sols.Count > 0;
            
            // act
            bool result = board.ExistsSolution(_context);
            
            // assert
            Assert.AreEqual(expected, result);
        }
        
        [TestCase(Board3X3)]
        [TestCase(Board5X5)]
        public void ExistsMatchInPosition_Test(int boardIndex)
        {
            // arrange
            var board = boards[boardIndex];
            var matchs = matches[boardIndex];
            var positions = board.MainLayer.GetPositions();
            
            foreach (var match in matchs)
            {
                // act
                var result = board.ExistsMatchInPosition(_context, match);
                // assert
                Assert.IsTrue(result);
                positions.Remove(match);
            }
            
            foreach (var position in positions)
            {
                // act
                var result = board.ExistsMatchInPosition(_context, position);
                // assert
                Assert.IsFalse(result);
            }
        }
        
        [TestCase(Board3X3)]
        [TestCase(Board5X5)]
        public void ExistsMatch_Test(int boardIndex)
        {
            // arrange
            var board = boards[boardIndex];
            var matchs = matches[boardIndex];
            bool expected = matchs.Count > 0;
            
            // act
            bool result = board.ExistsMatch(_context);
            
            // assert
            Assert.AreEqual(expected, result);
        }

        [TestCase(Board3X3)]
        [TestCase(Board5X5)]
        public void GetMatches_Test(int boardIndex)
        {
            // arrange
            var board = boards[boardIndex];
            var matchs = matchesObjects[boardIndex];
            
            // act
            var result = board.GetMatches(_context);
            
            // assert
            Assert.AreEqual(matchs.Count, result.Count);
            foreach (var match in result)
            {
                Assert.Contains(match, matchs);
            }
        }

        [TestCase(Board3X3)]
        [TestCase(Board5X5)]
        public void GetAdjacentInPosition_Test(int boardIndex)
        {
            // arrange
            var board = boards[boardIndex];
            var someAdjacent = adjacents[boardIndex];
            
            // act
            foreach (var (position, adj) in someAdjacent)
            {
                var result = new List<TokenData>();
                board.GetAdjacentInPosition(_context, position, result);
                // assert
                Assert.AreEqual(adj.Count, result.Count);
                foreach (var adjacentResult in result)
                {
                    Assert.Contains(adjacentResult, adj);
                }
            }
        }
        
        [TestCase(Board3X3)]
        [TestCase(Board5X5)]
        public void GetMatchesInPosition_Test(int boardIndex)
        {
            // arrange
            var board = boards[boardIndex];
            var someMatches = potentialMatches[boardIndex];
            
            // act
            foreach (var (position, matchs) in someMatches)
            {
                var result = new List<TokenData>();
                board.GetMatchesInPosition(_context, position, result);
                // assert
                Assert.AreEqual(matchs.Count, result.Count);
                foreach (var matchResult in result)
                {
                    Assert.Contains(matchResult, matchs);
                }
            }
        }
        
        [TestCase(Board3X3)]
        [TestCase(Board5X5)]
        public void GetSolutionsInPosition_Test(int boardIndex)
        {
            // arrange
            var board = boards[boardIndex];
            var someSolutions = potentialSolutions[boardIndex];
            
            // act
            foreach (var (position, soluts) in someSolutions)
            {
                var result = new List<TokenData>();
                board.GetSolutionsInPosition(_context, position, result);
                // assert
                Assert.AreEqual(soluts.Count, result.Count);
                foreach (var solutionResult in result)
                {
                    Assert.Contains(solutionResult, soluts);
                }
            }
        }
        
        [Test]
        public void GetSolutionsInPosition_WhenThereIsNoTokenToSwap_Test()
        {
            // arrange
            var board = boards[Board5X5];
            board.MainLayer.RemoveTokenAt(new Vector2Int(0, 4));
            board.MainLayer.RemoveTokenAt(new Vector2Int(2, 4));
            
            // act
            var soluts = new List<TokenData>();
            board.GetSolutionsInPosition(_context, new Vector2Int(1, 4), soluts);

            Assert.IsEmpty(soluts);
        }

        [TestCase("Assets/Match3/Tests/Editor/TestLevels/test-empty-9x9-2.asset")]
        [TestCase("Assets/Match3/Tests/Editor/TestLevels/test-empty-9x9-3.asset")]
        [TestCase("Assets/Match3/Tests/Editor/TestLevels/test-empty-9x9-4.asset")]
        [TestCase("Assets/Match3/Tests/Editor/TestLevels/test-empty-9x9-5.asset")]
        [TestCase("Assets/Match3/Tests/Editor/TestLevels/test-empty-9x9-6.asset")]
        public void WhenPopulatingEmptyLevel_ExistsAtLeastOneSolution(string levelPath)
        {
            // arrange
            int tries = 10;
            for (int i = 0; i < tries; i++)  // try several times to avoid false-positives due to randomness
            {
                // act
                var gameController = EditorTestsUtils.GetControllerFromLevelPath(levelPath);
                
                // assert
                bool existsSolution = gameController.Board.ExistsSolution(_context);
                Assert.IsTrue(existsSolution);
            }
        }
        
        [TestCase("Assets/Match3/Tests/Editor/TestLevels/test-empty-9x9-2.asset")]
        [TestCase("Assets/Match3/Tests/Editor/TestLevels/test-empty-9x9-3.asset")]
        [TestCase("Assets/Match3/Tests/Editor/TestLevels/test-empty-9x9-4.asset")]
        [TestCase("Assets/Match3/Tests/Editor/TestLevels/test-empty-9x9-5.asset")]
        [TestCase("Assets/Match3/Tests/Editor/TestLevels/test-empty-9x9-6.asset")][Test]
        public void WhenPopulatingEmptyLevel_DontExistsMatches(string levelPath)
        {
            // arrange
            int tries = 10;
            for (int i = 0; i < tries; i++)  // try several times to avoid false-positives due to randomness
            {
                // act
                var gameController = EditorTestsUtils.GetControllerFromLevelPath(levelPath);
                
                // assert
                bool existsMatch = gameController.Board.ExistsMatch(_context);
                Assert.IsFalse(existsMatch);
            }
        }
    }
}