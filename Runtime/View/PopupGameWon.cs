using System.Threading;
using System.Threading.Tasks;
using AsyncUtils;
using Match3.Core.GameDataExtraction;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.View
{
    public class PopupGameWon : AsyncPopupInitializable<GameData>
    {
        [SerializeField] private Button _continueButton;
        
        public override async Task Show(CancellationToken ct)
        {
            await AsyncUtils.Utils.WaitPressButtonAsync(_continueButton, ct);
        }

        public override void Initialize(GameData popupData)
        {
            
        }
    }
}