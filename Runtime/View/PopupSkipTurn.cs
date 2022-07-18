using System.Threading;
using System.Threading.Tasks;
using AsyncUtils;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.View
{
    public class PopupSkipTurn : AsyncPopup
    {
        [SerializeField] private Button _skipButton;
        
        public override async Task Show(CancellationToken ct)
        {
            await AsyncUtils.Utils.WaitPressButtonAsync(_skipButton, ct);
        }

        public override void Initialize()
        {
            // nothing to initialize
        }
    }
}