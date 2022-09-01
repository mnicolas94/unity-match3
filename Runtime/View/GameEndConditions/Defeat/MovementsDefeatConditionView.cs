using Match3.Core.GameDataExtraction;
using Match3.Core.GameEndConditions.Defeat;
using TMPro;
using UnityEngine;

namespace Match3.View.GameEndConditions.Defeat
{
    public class MovementsDefeatConditionView : DefeatConditionView<MovementsDefeatEvaluator>
    {
        [SerializeField] private TextMeshProUGUI movementsText;

        protected override void SetupUi(MovementsDefeatEvaluator defeatEvaluator)
        {
            SetRemainingMovementsUi(defeatEvaluator.Movements);
        }

        protected override void UpdateUi(MovementsDefeatEvaluator defeatEvaluator)
        {
            int remainingMovements = defeatEvaluator.GetRemainingMovements();
            SetRemainingMovementsUi(remainingMovements);
        }

        private void SetRemainingMovementsUi(int remainingMovements)
        {
            movementsText.text = remainingMovements.ToString();
        }
    }
}