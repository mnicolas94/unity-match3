using System.Threading;
using System.Threading.Tasks;
using Match3.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.View
{
    public class PopupGameWon : PopupGameEnded
    {
        [SerializeField] private Button _continueButton;
        
        public override async Task Show(CancellationToken ct)
        {
            await AsyncUtils.Utils.WaitPressButtonAsync(_continueButton, ct);
        }

        public override void Initialize((GameController, bool) popupData)
        {
            var (controller, victory) = popupData;
            // TODO
        }
    }
}