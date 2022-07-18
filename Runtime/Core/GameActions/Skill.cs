using Match3.Core.GameActions.Actions;
using UnityEngine;
using Utils.ModelView;

namespace Match3.Core.GameActions
{
    [CreateAssetMenu(fileName = "Skill", menuName = "Facticus/Match3/Skills/Skill")]
    public class Skill : ScriptableObject, IModel
    {
        [SerializeField] private string skillName;
        [SerializeField] private Sprite skillImage;
        [SerializeReference, SubclassSelector] private IGameAction _gameAction;
        [SerializeReference, SubclassSelector] private SkillCostBase _skillCost;

        public string SkillName => skillName;

        public Sprite SkillImage => skillImage;

        public IGameAction GameAction => _gameAction;
        
        public ApplySkillCostAction AppliesCostGameAction => new ApplySkillCostAction(_gameAction, this);

        public SkillCostBase SkillCost => _skillCost;
    }
}