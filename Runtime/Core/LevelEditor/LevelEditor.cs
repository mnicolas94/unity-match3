using System.Collections.Generic;
using Match3.Core.Levels;
using Match3.Core.SerializableTuples;
using Match3.Core.Tiles;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utils.Attributes;
using Utils.Extensions;

namespace Match3.Core.LevelEditor
{
    public class LevelEditor : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField] private Tilemap _shapeLayer;
        [SerializeField] private Tilemap _mainLayer;
        [SerializeField] private Transform _bottomLayersContainer;
        [SerializeField] private Transform _topLayersContainer;
        
        [Space]

        [SerializeField] [SortingLayer] private string _bottomLayerSortLayer;
        [SerializeField] [SortingLayer] private string _topLayerSortLayer;

        [SerializeField] private TileBase _shapeTile;
        [SerializeField] private Sprite tokenSourceSprite;
        [SerializeField] private Sprite _prohibitedPositionSprite;

        [NaughtyAttributes.Button]
        private Tilemap AddTopLayer()
        {
            return CreateTileMap(_topLayersContainer, _topLayerSortLayer);
        }
        
        [NaughtyAttributes.Button]
        private Tilemap AddBottomLayer()
        {
            return CreateTileMap(_bottomLayersContainer, _bottomLayerSortLayer);
        }
        
        public Level CreateLevel()
        {
            var level = ScriptableObject.CreateInstance<Level>();
            OverwriteLevel(level, false);
            return level;
        }

        public void OverwriteLevel(Level level, bool undoSupport = true)
        {
            CenterTileMaps();
            var boardShape = GetBoardShape();
            var tokenSourcesPositions = GetTokenSources();
            var tokensPositions = GetLayerFromTileMap(_mainLayer);
            var bottomLayers = GetLayersListFromTileMaps(GetBottomTileMaps());
            var topLayers = GetLayersListFromTileMaps(GetTopTileMaps());
            var prohibitedPositions = GetProhibitedPositions(_mainLayer);

            if (undoSupport)
                Undo.RecordObject(level, "Overwrite level with editor");
            
            Level.SetEditorData(
                level,
                boardShape,
                tokensPositions,
                bottomLayers,
                topLayers,
                tokenSourcesPositions,
                prohibitedPositions);
            
            EditorUtility.SetDirty(level);
        }

        public void InitializeFromLevel(Level level)
        {
            Clear();
            // add shape tiles
            foreach (var position in level.BoardShape.Tiles)
            {
                _shapeLayer.SetTile((Vector3Int) position, _shapeTile);
            }
            
            // add token sources
            foreach (var (position, tokenSource) in level.TokenSources)
            {
                var tile = TileTokenSource.CreateFromSprite(tokenSourceSprite);
                _mainLayer.SetTile((Vector3Int) position, tile);
            }

            // add main layer's tokens
            foreach (var (position, tokenData) in level.InitiallyPositionedTokens)
            {
                var tile = TileToken.CreateFromTokenData(tokenData);
                _mainLayer.SetTile((Vector3Int) position, tile);
            }
            
            // add bottom layers' tokens
            foreach (var layer in level.BottomLayersTokens)
            {
                var tileMapLayer = AddBottomLayer();
                foreach (var (position, tokenData) in layer)
                {
                    var tile = TileToken.CreateFromTokenData(tokenData);
                    tileMapLayer.SetTile((Vector3Int) position, tile);
                }
            }
            
            // add top layers' tokens
            foreach (var layer in level.TopLayersTokens)
            {
                var tileMapLayer = AddTopLayer();
                foreach (var (position, tokenData) in layer)
                {
                    var tile = TileToken.CreateFromTokenData(tokenData);
                    tileMapLayer.SetTile((Vector3Int) position, tile);
                }
            }
            
            // add prohibited positions
            foreach (var position in level.ProhibitedPositions)
            {
                var tile = TileProhibitedPosition.CreateFromSprite(_prohibitedPositionSprite);
                _mainLayer.SetTile((Vector3Int) position, tile);
            }
        }

        [NaughtyAttributes.Button]
        private void Clear()
        {
            _shapeLayer.ClearAllTiles();
            _mainLayer.ClearAllTiles();
            _bottomLayersContainer.ClearChildren();
            _topLayersContainer.ClearChildren();
        }

        [NaughtyAttributes.Button("Center")]
        private void CenterTileMaps()
        {
            var offset = _shapeLayer.Center();
            _mainLayer.MoveByOffset(offset);
            GetBottomTileMaps().ForEach(tm => tm.MoveByOffset(offset));
            GetTopTileMaps().ForEach(tm => tm.MoveByOffset(offset));
        }

        [NaughtyAttributes.Button]
        private void MirrorX()
        {
            _shapeLayer.MirrorHorizontally();
            _mainLayer.MirrorHorizontally();
            GetBottomTileMaps().ForEach(tm => tm.MirrorHorizontally());
            GetTopTileMaps().ForEach(tm => tm.MirrorHorizontally());
        }
        
        [NaughtyAttributes.Button]
        private void MirrorY()
        {
            _shapeLayer.MirrorVertically();
            _mainLayer.MirrorVertically();
            GetBottomTileMaps().ForEach(tm => tm.MirrorVertically());
            GetTopTileMaps().ForEach(tm => tm.MirrorVertically());
        }
        
        [NaughtyAttributes.Button("Rotate 90")]
        private void Rotate90()
        {
            _shapeLayer.Rotate90();
            _mainLayer.Rotate90();
            GetBottomTileMaps().ForEach(tm => tm.Rotate90());
            GetTopTileMaps().ForEach(tm => tm.Rotate90());
        }
        
        [NaughtyAttributes.Button("Rotate -90")]
        private void Rotate270()
        {
            _shapeLayer.Rotate270();
            _mainLayer.Rotate270();
            GetBottomTileMaps().ForEach(tm => tm.Rotate270());
            GetTopTileMaps().ForEach(tm => tm.Rotate270());
        }

        [NaughtyAttributes.Button]
        private void Validate()
        {
            var level = CreateLevel();
            Level.ValidateFromEditorState(level);
        }

        private List<PositionTokenSource> GetTokenSources()
        {
            var tokenSourcesPositions = new List<PositionTokenSource>();
            foreach (var tilePosition in _mainLayer.GetTilePositions())
            {
                var tile = _mainLayer.GetTile(tilePosition);
                var position = (Vector2Int) tilePosition;

                if (tile is TileTokenSource tileTokenSource)
                {
                    var tokeSource = tileTokenSource.GetTokenSource();
                    tokenSourcesPositions.Add(new PositionTokenSource
                    {
                        position = position,
                        tokenSource = tokeSource
                    });
                }
            }

            return tokenSourcesPositions;
        }

        private List<Tilemap> GetBottomTileMaps()
        {
            var tileMaps = _bottomLayersContainer.GetComponentsInChildren<Tilemap>();
            return new List<Tilemap>(tileMaps);
        }
        
        private List<Tilemap> GetTopTileMaps()
        {
            var tileMaps = _topLayersContainer.GetComponentsInChildren<Tilemap>();
            return new List<Tilemap>(tileMaps);
        }

        private List<PositionTokenData> GetLayerFromTileMap(Tilemap tileMap)
        {
            var tokensPositions = new List<PositionTokenData>();
            foreach (var tilePosition in tileMap.GetTilePositions())
            {
                var tile = tileMap.GetTile(tilePosition);
                var position = (Vector2Int) tilePosition;
                if (tile is TileToken tileToken)
                {
                    var token = tileToken.TokenData;
                    tokensPositions.Add(new PositionTokenData
                    {
                        position = position,
                        tokenData = token
                    });
                }
            }

            return tokensPositions;
        }

        private List<PositionTokenDataSerializableList> GetLayersListFromTileMaps(List<Tilemap> tileMaps)
        {
            var layers = new List<PositionTokenDataSerializableList>();
            foreach (var tileMap in tileMaps)
            {
                var layer = GetLayerFromTileMap(tileMap);
                layers.Add(layer);
            }

            return layers;
        }

        private List<Vector2Int> GetProhibitedPositions(Tilemap tileMap)
        {
            var positions = new List<Vector2Int>();
            foreach (var tilePosition in tileMap.GetTilePositions())
            {
                var position = (Vector2Int) tilePosition;
                var tile = tileMap.GetTile(tilePosition);
                if (tile is TileProhibitedPosition)
                {
                    positions.Add(position);
                }
            }

            return positions;
        }

        private BoardShape GetBoardShape()
        {
            return BoardShape.FromTileMap(_shapeLayer);
        }

        private Tilemap CreateTileMap(Transform parent, string sortLayer)
        {
            int index = parent.childCount;
            var go = new GameObject($"Layer {index + 1}");
            var tilemap = go.AddComponent<Tilemap>();
            var tileMapRenderer = go.AddComponent<TilemapRenderer>();
            tileMapRenderer.sortingLayerName = sortLayer;
            tileMapRenderer.sortingOrder = index;
            go.transform.SetParent(parent, false);

            return tilemap;
        }
        
#endif
    }
}