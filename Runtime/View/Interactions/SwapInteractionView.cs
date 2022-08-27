using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Match3.Core;
using Match3.Core.GameActions.Interactions;
using Match3.Core.Gravity;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Utils.Input;

namespace Match3.View.Interactions
{
    public class SwapInteractionView : InteractionViewBase<SwapInteraction>
    {
        [SerializeField] private Camera _camera;
        private BoardView _boardView;
        private InputAction _clickAction;
        private InputAction _pointAction;
        
        private bool _clickIsDown;
        private bool _swap;
        private bool _firstTokenClicked;
        private Vector2Int _firstPosition;
        private Token _firstToken;
        private TokenView _firstTokenView;
        private Vector2Int _secondPosition;

        private List<RaycastResult> _raycastBuffer;
        
        private void OnDisable()
        {
            _clickAction.Disable();
            _pointAction.Disable();
        }

        public override void Initialize()
        {
            _boardView = FindObjectOfType<BoardView>();
            _clickAction = InputActionUtils.GetClickAction();
            _pointAction = InputActionUtils.GetPointAction();

            _clickAction.performed += OnClick;
            _pointAction.performed += OnPointerMove;
            
            _raycastBuffer = new List<RaycastResult>();
            
            ResetState();
        }

        protected override async Task<(SwapInteraction, bool)> WaitInteractionBaseAsync(CancellationToken ct)
        {
            ResetState();
            _clickAction.Enable();
            _pointAction.Enable();

            while (!_swap && !ct.IsCancellationRequested)
                await Task.Yield();
            
            _clickAction.Disable();
            _pointAction.Disable();

            bool success = _swap;
            ResetState();
            
            var interaction = new SwapInteraction(_firstPosition, _secondPosition);
            return (interaction, success);
        }

        private void OnClick(InputAction.CallbackContext context)
        {
            _clickIsDown = context.ReadValue<float>() > 0.5f;
            bool anythingRaycasted = RaycastsAnything();

            bool tokenClicked = false;
            
            if (_clickIsDown && !anythingRaycasted)
            {
                var (position, token, tokenView, exists) = GetInfoAtPointer();
                if (exists && CanMove(token))
                {
                    _firstTokenClicked = true;
                    _firstPosition = position;
                    _firstToken = token;
                    _firstTokenView = tokenView;
                    tokenClicked = true;
                }
            }
            
            if (!tokenClicked)
            {
                _firstTokenClicked = false;
            }
        }

        private bool RaycastsAnything()
        {
            var pos = _pointAction.ReadValue<Vector2>();
            var eventData = new PointerEventData(EventSystem.current)
            {
                position = pos
            };
            EventSystem.current.RaycastAll(eventData, _raycastBuffer);
            return _raycastBuffer.Count > 0;
        }

        private void OnPointerMove(InputAction.CallbackContext context)
        {
            if (_clickIsDown && _firstTokenClicked)
            {
                var (secondPosition, secondToken, secondTokenView, exists) = GetInfoAtPointer();
                if (exists && CanMove(secondToken))
                {
                    bool isDifferent = secondToken != _firstToken;
                    if (isDifferent)
                    {
                        _secondPosition = secondPosition;
                        bool adjacent = Board.ArePositionsAdjacent(_firstPosition, _secondPosition);

                        _swap = adjacent;
                    }
                }
            }
        }

        private (Vector2Int, Token, TokenView, bool) GetInfoAtPointer()
        {
            var board = _boardView.Board;
            var position = GetBoardPositionAtPointer();
            bool existsToken = board.MainLayer.ExistsTokenAt(position);
            if (existsToken)
            {
                var token = board.MainLayer.GetTokenAt(position);
                var tokenView = _boardView.GetTokenView(token);
                return (position, token, tokenView, true);
            }

            return (default, default, default, false);
        }
        
        private Vector2Int GetBoardPositionAtPointer()
        {
            var screenPosition = _pointAction.ReadValue<Vector2>();
            var worldPosition = _camera.ScreenToWorldPoint(screenPosition);
            var boardPosition = _boardView.GetWorldToBoardPosition(worldPosition);
            return boardPosition;
        }

        private bool CanMove(Token token)
        {
            var board = _boardView.Board;
            var position = board.MainLayer.GetPositionOfToken(token);
            bool canMove = GravityUtils.CanMoveFrom(board, position);
            return canMove;
        }

        private void ResetState()
        {
            _clickIsDown = false;
            _firstTokenClicked = false;
            _swap = false;
        }
    }
}