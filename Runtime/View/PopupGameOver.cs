using System.Threading;
using System.Threading.Tasks;
using AsyncUtils;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.View
{
    public class PopupGameOver : AsyncPopupReturnable<bool>
    {
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _restartButton;
        
        public override async Task<bool> Show(CancellationToken ct)
        {
            var pressedButton = await AsyncUtils.Utils.WaitFirstButtonPressedAsync(ct, _continueButton, _restartButton);
            bool restart = pressedButton == _restartButton;
            return restart;
        }

        public override void Initialize()
        {
        }
    }
}