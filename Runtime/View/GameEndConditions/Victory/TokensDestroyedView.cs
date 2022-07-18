using Match3.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.View.GameEndConditions.Victory
{
    public class TokensDestroyedView : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI countText;

        public void SetupUi(TokenData tokenData, int countRequirement)
        {
            image.sprite = tokenData.TokenSprite;
            UpdateUi(countRequirement);            
        }

        public void UpdateUi(int remainingCount)
        {
            countText.text = remainingCount.ToString();
        }
    }
}