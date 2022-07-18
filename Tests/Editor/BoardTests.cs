using System;
using System.Collections.Generic;
using Match3.Core;
using NUnit.Framework;
using UnityEngine;

namespace Match3.Tests.Editor
{
    public class BoardTests
    {
        private Board board;
        private BoardShape boardShape;
        
        [SetUp]
        public void Setup()
        {
            var positions = new List<Vector2Int>
            {
                new Vector2Int(0, 0),
                new Vector2Int(0, 1),
                new Vector2Int(0, 2),
                new Vector2Int(1, 0),
                new Vector2Int(1, 1),
                new Vector2Int(1, 2),
                new Vector2Int(2, 0),
                new Vector2Int(2, 1),
                new Vector2Int(2, 2),
            };
            boardShape = new BoardShape(positions);
            board = new Board(boardShape);
        }

        [TestCase(0, 0)]
        [TestCase(0, 2)]
        [TestCase(1, 1)]
        [TestCase(2, 0)]
        [TestCase(2, 2)]
        public void AddTokenAt_Test(int x, int y)
        {
            // arrange
            var token = new Token(null);
            var position = new Vector2Int(x, y);

            // act
            board.MainLayer.AddTokenAt(token, position);

            // assert
            bool exists = board.MainLayer.ExistsTokenAt(position);
            Assert.IsTrue(exists);
        }
        
        [TestCase(-1, 0)]
        [TestCase(0, 3)]
        [TestCase(1, 4)]
        [TestCase(2, -5)]
        [TestCase(2, 20)]
        public void AddTokenAt_OutOfBoardShape_Test(int x, int y)
        {
            // arrange
            var token = new Token(null);
            var position = new Vector2Int(x, y);

            // act and assert
            Assert.Throws<ArgumentException>(() => board.MainLayer.AddTokenAt(token, position));
        }

        [Test]
        public void GetTokenAt_Test()
        {
            // arrange
            var token = new Token(null);
            var position = new Vector2Int(0, 1);
            board.MainLayer.AddTokenAt(token, position);
            
            // act
            var result = board.MainLayer.GetTokenAt(position);

            // assert
            Assert.AreEqual(token, result);
        }
        
        [Test]
        public void GetPositionOfToken_Test()
        {
            // arrange
            var token = new Token(null);
            var position = new Vector2Int(0, 1);
            board.MainLayer.AddTokenAt(token, position);
            
            // act
            var result = board.MainLayer.GetPositionOfToken(token);

            // assert
            Assert.AreEqual(position, result);
        }
        
        [Test]
        public void ExistsTokenAt_Test()
        {
            // arrange
            var token = new Token(null);
            var position = new Vector2Int(0, 1);
            
            // false position assert
            Assert.IsFalse(board.MainLayer.ExistsTokenAt(position));
            
            // act
            board.MainLayer.AddTokenAt(token, position);
            var result = board.MainLayer.ExistsTokenAt(position);

            // assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void ExistsToken_Test()
        {
            // arrange
            var token = new Token(null);
            var position = new Vector2Int(0, 1);
            
            // false position assert
            Assert.IsFalse(board.MainLayer.ExistsToken(token));
            
            // act
            board.MainLayer.AddTokenAt(token, position);
            var result = board.MainLayer.ExistsToken(token);

            // assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void RemoveTokenAt_Test()
        {
            // arrange
            var token = new Token(null);
            var position = new Vector2Int(0, 1);
            board.MainLayer.AddTokenAt(token, position);
            
            // act
            board.MainLayer.RemoveTokenAt(position);

            // assert
            bool exists = board.MainLayer.ExistsTokenAt(position);
            Assert.IsFalse(exists);
        }
        
        [Test]
        public void RemoveToken_Test()
        {
            // arrange
            var token = new Token(null);
            var position = new Vector2Int(0, 1);
            board.MainLayer.AddTokenAt(token, position);
            
            // act
            board.MainLayer.RemoveToken(token);

            // assert
            bool exists = board.MainLayer.ExistsToken(token);
            Assert.IsFalse(exists);
        }
        
        [Test]
        public void MoveTokenTo_Test()
        {
            // arrange
            var token = new Token(null);
            var positionA = new Vector2Int(0, 1);
            var positionB = new Vector2Int(1, 2);
            board.MainLayer.AddTokenAt(token, positionA);
            
            // false position assert
            Assert.IsFalse(board.MainLayer.ExistsTokenAt(positionB));
            
            // act
            board.MoveTokenTo(positionA, positionB);

            // assert
            Assert.IsFalse(board.MainLayer.ExistsTokenAt(positionA));
            Assert.IsTrue(board.MainLayer.ExistsTokenAt(positionB));
        }
        
        [Test]
        public void SwapPositions_Test()
        {
            // arrange
            var tokenA = new Token(null);
            var tokenB = new Token(null);
            var positionA = new Vector2Int(0, 1);
            var positionB = new Vector2Int(1, 2);
            board.MainLayer.AddTokenAt(tokenA, positionA);
            board.MainLayer.AddTokenAt(tokenB, positionB);
            
            // act
            board.SwapPositions(positionA, positionB);

            // assert
            var tokenAtA = board.MainLayer.GetTokenAt(positionA);
            var tokenAtB = board.MainLayer.GetTokenAt(positionB);
            Assert.AreEqual(tokenA, tokenAtB);
            Assert.AreEqual(tokenB, tokenAtA);
        }
        
        [Test]
        public void GetPositions_Test()
        {
            // arrange
            var token = new Token(null);
            var positions = new List<Vector2Int>
            {
                new Vector2Int(0, 2),
                new Vector2Int(1, 1),
                new Vector2Int(0, 1),
                new Vector2Int(2, 0),
            };
            foreach (var position in positions)
            {
                board.MainLayer.AddTokenAt(token, position);
            }
            
            // act
            var resultPositions = board.MainLayer.GetPositions();

            // assert
            Assert.IsTrue(resultPositions.Count == positions.Count);
            foreach (var position in resultPositions)
            {
                bool contains = positions.Contains(position);
                Assert.IsTrue(contains);
            }
        }
        
        [Test]
        public void GetTokenPositions_Test()
        {
            // arrange
            var positions = new List<(Vector2Int, Token)>
            {
                (new Vector2Int(0, 2), new Token(null)),
                (new Vector2Int(1, 1), new Token(null)),
                (new Vector2Int(0, 1), new Token(null)),
                (new Vector2Int(2, 0), new Token(null)),
            };
            foreach (var (position, token) in positions)
            {
                board.MainLayer.AddTokenAt(token, position);
            }
            
            // act
            var resultPositions = board.MainLayer.GetTokenPositions();

            // assert
            Assert.IsTrue(resultPositions.Count == positions.Count);
            foreach (var tokenPosition in resultPositions)
            {
                bool contains = positions.Contains(tokenPosition);
                Assert.IsTrue(contains);
            }
        }
        
        [Test]
        public void GetEmptyPositions_Test()
        {
            // arrange
            var token = new Token(null);
            var positions = new List<Vector2Int>
            {
                new Vector2Int(0, 2),
                new Vector2Int(1, 1),
                new Vector2Int(0, 1),
                new Vector2Int(2, 0),
            };
            foreach (var position in positions)
            {
                board.MainLayer.AddTokenAt(token, position);
            }
            
            // act
            var resultPositions = board.GetEmptyPositions();

            // assert
            foreach (var position in resultPositions)
            {
                bool contains = positions.Contains(position);
                Assert.IsFalse(contains);
            }
        }
        
        [TestCase(0, 0, 1, 0)]
        [TestCase(1, 1, 1, 0)]
        [TestCase(3, 2, 2, 2)]
        [TestCase(-24, 54, -24, 53)]
        public void ArePositionsAdjacent_Test(int x1, int y1, int x2, int y2)
        {
            // act
            var positionA = new Vector2Int(x1, y1);
            var positionB = new Vector2Int(x2, y2);

            // assert
            bool result = Board.ArePositionsAdjacent(positionA, positionB);
            Assert.IsTrue(result);
        }

        [TestCase(0, 0, -1, 0, true)]
        [TestCase(0, 0, 0, -1, true)]
        [TestCase(0, 0, 1, 0, false)]
        [TestCase(0, 0, 0, 1, false)]
        [TestCase(4, 4, 0, 1, true)]
        [TestCase(4, 4, 1, 0, true)]
        [TestCase(2, 3, 0, 1, true)]
        [TestCase(2, 3, 0, -1, false)]
        [TestCase(2, 1, 0, -1, true)]
        [TestCase(2, 1, 0, 1, false)]
        [TestCase(1, 2, -1, 0, true)]
        [TestCase(1, 2, 1, 0, false)]
        [TestCase(3, 2, 1, 0, true)]
        [TestCase(3, 2, -1, 0, false)]
        [TestCase(2, 2, 1, 0, false)]
        public void IsLimitInDirection_Test(int x, int y, int dx, int dy, bool expected)
        {
            // Arrange
            /*
             * O O   O O
             * O O O O O
             *   O   O
             * O O O O O
             * O O   O O
             */
            Vector2Int V(int vx, int vy)
            {
                return new Vector2Int(vx, vy);
            }
            var positions = new List<Vector2Int>
            {
                V(0, 4), V(1, 4), V(3, 4), V(4, 4),
                V(0, 3), V(1, 3), V(2, 3), V(3, 3), V(4, 3),
                V(1, 2), V(3, 2),
                V(0, 1), V(1, 1), V(2, 1), V(3, 1), V(4, 1),
                V(0, 0), V(1, 0), V(3, 0), V(4, 0),
            };
            boardShape = new BoardShape(positions);
            board = new Board(boardShape);

            // Act
            var position = V(x, y);
            var direction = V(dx, dy);
            bool actual =boardShape.IsLimitInDirection(position, direction);
            
            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
