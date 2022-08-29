using System.Linq;
using Match3.Core.SerializableDictionaries;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using Utils;

namespace Match3.Core.GameActions
{
    [CreateAssetMenu(fileName = "SkillsCountPersistent", menuName = "Facticus/Match3/Persistent/SkillsCountStorage", order = 0)]
    public class SkillsCountStorage : ScriptableObject
    {
        [SerializeField] private SkillCountDictionary _skillsCount;
        [SerializeField] private VoidEvent _onStorageChange;

        public VoidEvent OnStorageChange => _onStorageChange;

        public int GetSkillCount(Skill skill)
        {
            var dict = _skillsCount;
            return dict.ContainsKey(skill) ? dict[skill] : 0;
        }

        public void AddSkillCount(Skill skill, int count = 1)
        {
            var dict = _skillsCount;
            if (dict.ContainsKey(skill))
                dict[skill] += count;
            else
                dict.Add(skill, count);

            _onStorageChange.Raise();
        }
        
        public void AddAllSkillsCount(int count)
        {
            var dict = _skillsCount;
            foreach (var skill in dict.Keys.ToList())
            {
                AddSkillCount(skill, count);
            }
        }

        public bool ConsumeSkill(Skill skill)
        {
            if (GetSkillCount(skill) > 0)
            {
                AddSkillCount(skill, -1);
                return true;
            }

            return false;
        }
    }
}