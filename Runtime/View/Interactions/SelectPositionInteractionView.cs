using System.Threading;
using System.Threading.Tasks;
using Match3.Core.GameActions.Interactions;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils.Input;

namespace Match3.View.Interactions
{
    public class SelectPositionInteractionView : InteractionViewBase<SelectPositionInteraction>
    {
        [SerializeField] private Camera _camera;
        private BoardView _boardView;
        private InputAction _clickAction;
        private InputAction _pointAction;

        private bool _clicked;
        private Vector2Int _position;

        public override void Initialize()
        {
            _boardView = FindObjectOfType<BoardView>();

            _clickAction = InputActionUtils.GetTapAction();
            _pointAction = InputActionUtils.GetPointAction();
        }

        private void OnDisable()
        {
            _clickAction.Disable();
            _pointAction.Disable();
        }
        
        protected override async Task<(SelectPositionInteraction, bool)> WaitInteractionBaseAsync(CancellationToken ct)
        {
            // TODO highlight tiles
            _pointAction.Enable();
            await AsyncUtils.Utils.WaitForInputAction(_clickAction, ct);
            var screenPosition = _pointAction.ReadValue<Vector2>();
            _clickAction?.Disable();
            _pointAction?.Disable();
            
            // TODO un-highlight tiles

            var worldPosition = _camera.ScreenToWorldPoint(screenPosition);
            var boardPosition = _boardView.GetWorldToBoardPosition(worldPosition);
            
            var board = _boardView.Board;
            bool existsToken = board.ExistsTokenAnyLayerAt(boardPosition);
            
            if (!existsToken)
                return (null, false);
            
            var interaction = new SelectPositionInteraction(boardPosition);
            return (interaction, true);
        }
    }
}