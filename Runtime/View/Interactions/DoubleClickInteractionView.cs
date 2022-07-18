using System.Threading;
using System.Threading.Tasks;
using Match3.Core.GameActions.Interactions;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils.Input;

namespace Match3.View.Interactions
{
    public class DoubleClickInteractionView : InteractionViewBase<DoubleClickInteraction>
    {
        [SerializeField] private Camera _camera;
        private BoardView _boardView;
        private InputAction _doubleClickAction;
        private InputAction _pointAction;
        
        public override void Initialize()
        {
            _boardView = FindObjectOfType<BoardView>();
            _doubleClickAction = InputActionUtils.GetDoubleTapAction();
            _pointAction = InputActionUtils.GetPointAction();
        }

        private void OnDisable()
        {
            _doubleClickAction.Disable();
            _pointAction.Disable();
        }

        protected override async Task<(DoubleClickInteraction, bool)> WaitInteractionBaseAsync(CancellationToken ct)
        {
            _doubleClickAction.Enable();
            _pointAction.Enable();
            
            await AsyncUtils.Utils.WaitForInputAction(_doubleClickAction, ct);
            
            var screenPosition = _pointAction.ReadValue<Vector2>();
            _doubleClickAction?.Disable();
            _pointAction?.Disable();
            
            var worldPosition = _camera.ScreenToWorldPoint(screenPosition);
            var boardPosition = _boardView.GetWorldToBoardPosition(worldPosition);
            
            var board = _boardView.Board;
            
            bool existsToken = board.ExistsTokenAnyLayerAt(boardPosition);
            
            if (!existsToken)
                return (null, false);
            
            var interaction = new DoubleClickInteraction(boardPosition);
            return (interaction, true);
        }
    }
}