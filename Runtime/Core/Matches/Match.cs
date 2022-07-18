using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using Utils.Extensions;

namespace Match3.Core.Matches
{
    [Serializable]
    public class Match
    {
        [SerializeField] private List<Vector2Int> _positions;
        [SerializeField] private int _intersections;
        [SerializeField] private int _maxLineCount;

        public ReadOnlyCollection<Vector2Int> Positions => _positions.AsReadOnly();

        public int Intersections => _intersections;

        public Match(List<Vector2Int> positions)
        {
            _positions = positions;
            _intersections = 0;
            _maxLineCount = positions.Count;
        }

        public static Match Merge(Match a, Match b)
        {
            Match match = null;
            if (a.Equals(b))
            {
                var positions = new List<Vector2Int>(a._positions);
                match = new Match(positions);
            }
            else if (a.Intersects(b))
            {
                var positions = new List<Vector2Int>(a._positions);
                positions.AddRangeIfNotExists(b._positions);
                match = new Match(positions);
                match._intersections++;
                match._maxLineCount = Math.Max(a._maxLineCount, b._maxLineCount);
            }

            return match;
        }

        public List<Vector2Int> IntersectionsPositions(List<Vector2Int> positions)
        {
            var intersections = new List<Vector2Int>();
            
            foreach (var position in _positions)
            {
                if (positions.Contains(position))
                {
                    intersections.Add(position);
                }
            }

            return intersections;
        }
        
        public List<Vector2Int> IntersectionsPositions(Match other)
        {
            return IntersectionsPositions(other._positions);
        }
        
        public bool Intersects(Match other)
        {
            foreach (var position in _positions)
            {
                if (other._positions.Contains(position))
                {
                    return true;
                }
            }

            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj is Match match)
            {
                return Equals(match);
            }
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_positions != null ? _positions.GetHashCode() : 0) * 397) ^ _intersections;
            }
        }

        private bool Equals(Match other)
        {
            if (other._intersections != _intersections)
            {
                return false;
            }

            if (other._positions.Count != _positions.Count)
            {
                return false;
            }

            foreach (var position in _positions)
            {
                if (!other._positions.Contains(position))
                {
                    return false;
                }
            }
            
            return true;
        }
    }
}