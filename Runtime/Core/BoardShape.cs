using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Match3.Core.Tiles;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utils.Extensions;

namespace Match3.Core
{
    [Serializable]
    public struct BoardShape
    {
        [SerializeField] private List<Vector2Int> _tiles;

        [NonSerialized] private RectInt _bounds;
        [NonSerialized] private bool _boundsComputed;
        
        public ReadOnlyCollection<Vector2Int> Tiles => _tiles.AsReadOnly();

        public BoardShape(List<Vector2Int> tiles)
        {
            _tiles = tiles;
            _bounds = new RectInt();
            _boundsComputed = false;
        }

        public static BoardShape FromTileMap(Tilemap tileMap)
        {
            var positions = tileMap.GetTilePositions();
            var positions2d = new List<Vector2Int>();
            foreach (var position in positions)
            {
                var tile = tileMap.GetTile(position);
                if (!(tile is TileTokenSource))
                {
                    positions2d.Add((Vector2Int) position);
                }
            }
            return new BoardShape(positions2d);
        }

        public static BoardShape Join(BoardShape a, BoardShape b)
        {
            var tiles = new List<Vector2Int>(a._tiles);
            tiles.AddRangeIfNotExists(b._tiles);
            var newBoardShape = new BoardShape(tiles);
            return newBoardShape;
        }
        
        public bool ExistsPosition(Vector2Int position)
        {
            return _tiles.Contains(position);
        }

        public RectInt GetBounds()
        {
            if (!_boundsComputed)
                ComputeBounds();
            return _bounds;
        }

        /// <summary>
        /// Whether this position is the last one in the specified direction.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public bool IsLimitInDirection(Vector2Int position, Vector2Int direction)
        {
            if (direction.sqrMagnitude != 1)
                throw new ArgumentException("Direction must be one of Vector2Int.Up, Vector2Int.Down," +
                                            $"Vector2Int.Left or Vector2Int.Right. Current argument is {direction}");
            
            var bounds = GetBounds();
            bool exists = ExistsPosition(position);
            bool inside = bounds.Contains(position);
            
            if (!exists)
                return false;
            
            while (inside)
            {
                position += direction;
                exists = ExistsPosition(position);
                inside = bounds.Contains(position);
                if (exists)
                    return false;
            }

            return true;
        }

        private void ComputeBounds()
        {
            int minX = Int32.MaxValue;
            int maxX = Int32.MinValue;
            int minY = Int32.MaxValue;
            int maxY = Int32.MinValue;
            for (int i = 0; i < _tiles.Count; i++)
            {
                var position = _tiles[i];
                minX = Math.Min(minX, position.x);
                maxX = Math.Max(maxX, position.x);
                minY = Math.Min(minY, position.y);
                maxY = Math.Max(maxY, position.y);
            }

            int width = maxX - minX + 1;
            int height = maxY - minY + 1;
            _bounds = new RectInt(minX, minY, width, height);
            _boundsComputed = true;
        }
        
    }
}