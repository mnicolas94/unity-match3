using System.Threading;
using System.Threading.Tasks;
using AsyncUtils;
using Match3.Core;
using Match3.Core.GameDataExtraction;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.View
{
    public class PopupGameWon : PopupGameEnded
    {
        [SerializeField] private Button _continueButton;
        [SerializeField] private string _goToScene;
        
        public override async Task Show(CancellationToken ct)
        {
            await AsyncUtils.Utils.WaitPressButtonAsync(_continueButton, ct);
            await LoadingUtils.LoadingUtils.LoadSceneAsync(_goToScene, null, null);
        }

        public override void Initialize((GameController, bool) popupData)
        {
            var (controller, victory) = popupData;
            // TODO
        }
    }
}