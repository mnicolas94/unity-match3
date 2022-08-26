using System.Threading;
using System.Threading.Tasks;
using AsyncUtils;
using Match3.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.View
{
    public class PopupGameOver : PopupGameEnded
    {
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _restartButton;

        public override async Task Show(CancellationToken ct)
        {
            var pressedButton = await AsyncUtils.Utils.WaitFirstButtonPressedAsync(ct, _continueButton, _restartButton);
            bool restart = pressedButton == _restartButton;
            if (restart)
            {
                await Task.Delay(200, ct);
                FindObjectOfType<GameControllerView>().RestartCurrentLevel();
            }
            else
            {
                var gcv = FindObjectOfType<GameControllerView>();
                gcv.ResumeCurrentGame();
            }
        }

        public override void Initialize((GameController, bool) popupData)
        {
            var (controller, victory) = popupData;
            // TODO
        }
    }
}