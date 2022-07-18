using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Match3.Core;
using Match3.Core.TurnSteps;
using Match3.View.TurnStepRenderers;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Tilemaps;
using Utils.Extensions;
using Utils.Serializables;

namespace Match3.View
{
    [Serializable]
    public class DataViewDictionary : SerializableDictionary<Token, TokenView>{}
    
    public class BoardView : MonoBehaviour
    {
        [SerializeField] private Grid grid;
        [SerializeField] private Tilemap boardShapeTileMap;
        [SerializeField] private TileBase boardShapeTile;
        [SerializeField] private bool _dualGrid;
        [SerializeField] private Transform tokensContainer;
        [SerializeField] private TokenView tokenPrefab;

        [Space]
        
        [SerializeField] private Transform renderersContainer;
        [SerializeField] private List<TurnStepRenderer> renderers;

        [SerializeField] [HideInInspector] private Board _board;
        [SerializeField] [HideInInspector] private DataViewDictionary dataViewMap = new DataViewDictionary();
        private TokenDataViewMap _dataViewMapWrapper;

        private ObjectPool<TokenView> _tokenViewsPool;
        
        public Board Board => _board;

        private void Awake()
        {
            _tokenViewsPool = new ObjectPool<TokenView>(
                CreateTokenView,
                (view) => view.gameObject.SetActive(true),
                (view) => view.gameObject.SetActive(false)
                );
            _dataViewMapWrapper = new TokenDataViewMap
            {
                Clear = Clear,
                AddTokenToMap = AddTokenToMap,
                GetTokenView = GetTokenView,
                GetTokenViewOrAddIfNotExists = GetTokenViewOrAddIfNotExists,
                GetTokenViewAt = GetTokenViewAt,
                MapHasToken = HasViewForToken,
                RemoveTokenFromMap = RemoveTokenFromMap
            };
            
            InitializeRenderers();
            ClearForce();
        }

        [NaughtyAttributes.Button]
        public void Clear()
        {
            if (Application.isPlaying)
            {
                foreach (var token in dataViewMap.Keys)
                {
                    var tokenView = dataViewMap[token];
                    _tokenViewsPool.Release(tokenView);
                }
            }
            else
            {
                tokensContainer.ClearChildren();
            }
            dataViewMap.Clear();
            boardShapeTileMap.ClearAllTiles();
        }

        /// <summary>
        /// Destroy all children tokens instead of just releasing them from the pool.
        /// </summary>
        private void ClearForce()
        {
            Clear();
            tokensContainer.ClearChildren();
        }

        public void UpdateView(Board board)
        {
            _board = board;
            Clear();
            PopulateBoardShapeTileMap();
            UpdateDataViewMap();
            UpdateTokensPositions();
        }

        public Vector2Int GetWorldToBoardPosition(Vector3 worldPosition)
        {
            Vector2Int boardPosition = (Vector2Int) grid.WorldToCell(worldPosition);
            return boardPosition;
        }
        
        public bool HasViewForToken(Token token)
        {
            return dataViewMap.ContainsKey(token);
        }
        
        public TokenView GetTokenView(Token token)
        {
            var tokenView = dataViewMap[token];
            return tokenView;
        }

        public TokenView GetTokenViewAt(Vector2Int position)
        {
            var token = _board.MainLayer.GetTokenAt(position);
            return GetTokenView(token);
        }

#region Turn steps rendering

        public async Task RenderTurnAsync(IEnumerable<TurnStep> turn, CancellationToken ct)
        {
            await RenderTurnStepsAsync(turn, ct);
            UpdateDataViewMap();
        }
        
        public async Task RenderTurnStepAsync(TurnStep turnStep, CancellationToken ct)
        {
            var filtered = renderers.FindAll(rend =>
            {
                bool isActive = rend.isActiveAndEnabled;
                bool canRender = rend.CanRenderStep(turnStep);
                return isActive && canRender;
            });
            var tasks = filtered.ConvertAll(rend => rend.RenderTurnStep(
                turnStep,
                grid,
                _board,
                _dataViewMapWrapper,
                ct
            ));
            await Task.WhenAll(tasks);
        }

        private async Task RenderTurnStepsAsync(IEnumerable<TurnStep> turn, CancellationToken ct)
        {
            foreach (var turnStep in turn)
            {
                await RenderTurnStepAsync(turnStep, ct);
            }
        }
        
#endregion

        private void InitializeRenderers()
        {
            var rends = renderersContainer.GetComponentsInChildren<TurnStepRenderer>();
            foreach (var rend in rends)
            {
                if (!renderers.Contains(rend))
                    renderers.Add(rend);
            }
        }
        
        private void PopulateBoardShapeTileMap()
        {
            boardShapeTileMap.ClearAllTiles();

            foreach (var position in _board.BoardShape.Tiles)
            {
                if (!_dualGrid)
                    boardShapeTileMap.SetTile((Vector3Int) position, boardShapeTile);
                else
                {
                    var pos = (Vector3Int) position * 2;
                    var topPos = pos + Vector3Int.up;
                    var rightPos = pos + Vector3Int.right;
                    var topRightPos = pos + Vector3Int.one + Vector3Int.back;
                    
                    if (!boardShapeTileMap.HasTile(pos))
                        boardShapeTileMap.SetTile(pos, boardShapeTile);
                    if (!boardShapeTileMap.HasTile(rightPos))
                        boardShapeTileMap.SetTile(rightPos, boardShapeTile);
                    if (!boardShapeTileMap.HasTile(topPos))
                        boardShapeTileMap.SetTile(topPos, boardShapeTile);
                    if (!boardShapeTileMap.HasTile(topRightPos))
                        boardShapeTileMap.SetTile(topRightPos, boardShapeTile);
                }
            }
        }

        private void UpdateTokensPositions()
        {
            foreach (var (position, token) in _board.GetTokenPositionsAllLayers())
            {
                var tokenView = dataViewMap[token];
                var worldPosition = grid.GetCellCenterWorld((Vector3Int) position);
                tokenView.transform.position = worldPosition;
            }
        }

        private void UpdateDataViewMap()
        {
            // add tokens not mapped
            foreach (var (position, token, layerIndex) in _board.GetTokenPositionsAllLayersWithLayerIndex())
            {
                if (!HasViewForToken(token))
                {
                    AddTokenToMap(token, layerIndex);
                }
            }

            // remove tokens that do not exist in board anymore
            var tokens = new List<Token>(dataViewMap.Keys);
            foreach (var token in tokens)
            {
                if (!_board.ExistsTokenInAnyLayer(token))
                {
                    RemoveTokenFromMap(token);
                }
            }
        }

        private TokenView CreateTokenView()
        {
            var tokenView = Instantiate(tokenPrefab, tokensContainer);
            tokenView.onDestroyed += () => _tokenViewsPool.Release(tokenView);
            return tokenView;
        }
        
        private TokenView AddTokenToMap(Token token, int layerIndex)
        {
            var tokenView = Application.isPlaying
                ? _tokenViewsPool.Get()
                : CreateTokenView();
            tokenView.Initialize(token, layerIndex);
            dataViewMap.Add(token, tokenView);
            return tokenView;
        }
        
        private void RemoveTokenFromMap(Token token)
        {
            if (!HasViewForToken(token))
                return;
            
            var tokenView = dataViewMap[token];
            dataViewMap.Remove(token);
            if (Application.isPlaying)
            {
                tokenView.Destroy();
            }
            else
            {
                DestroyImmediate(tokenView.gameObject);
            }
        }

        private TokenView GetTokenViewOrAddIfNotExists(Token token, int layerIndex = 0)
        {
            var tokenView = HasViewForToken(token)
                    ? GetTokenView(token)
                    : AddTokenToMap(token, layerIndex);
            return tokenView;
        }
    }
    
    public struct TokenDataViewMap
    {
        public Action Clear;
        public Func<Token, bool> MapHasToken;
        public Func<Token, int, TokenView> AddTokenToMap;
        public Action<Token> RemoveTokenFromMap;
        public Func<Token, TokenView> GetTokenView;
        public Func<Token, int, TokenView> GetTokenViewOrAddIfNotExists;
        public Func<Vector2Int, TokenView> GetTokenViewAt;
    }
}