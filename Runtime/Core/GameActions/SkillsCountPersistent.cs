using System.Linq;
using Match3.Core.SerializableDictionaries;
using UnityEngine;
using Utils;

namespace Match3.Core.GameActions
{
    [CreateAssetMenu(fileName = "SkillsCountPersistent", menuName = "Match3/Persistent/SkillsCountPersistent", order = 0)]
    public class SkillsCountPersistent : ScriptableObjectSingleton<SkillsCountPersistent>
    {
        [SerializeField] private SkillCountDictionary _skillsCount;

        public static int GetSkillCount(Skill skill)
        {
            var dict = Instance._skillsCount;
            return dict.ContainsKey(skill) ? dict[skill] : 0;
        }

        public static void AddSkillCount(Skill skill, int count = 1)
        {
            var dict = Instance._skillsCount;
            if (dict.ContainsKey(skill))
                dict[skill] += count;
            else
                dict.Add(skill, count);
        }
        
        public static void AddAllSkillsCount(int count)
        {
            var dict = Instance._skillsCount;
            foreach (var skill in dict.Keys.ToList())
            {
                AddSkillCount(skill, count);
            }
        }

        public static bool ConsumeSkill(Skill skill)
        {
            if (GetSkillCount(skill) > 0)
            {
                Instance._skillsCount[skill]--;
                return true;
            }

            return false;
        }
    }
}