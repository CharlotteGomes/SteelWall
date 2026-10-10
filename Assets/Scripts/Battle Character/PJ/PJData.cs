using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PJData : BattleCharData
{
    public string text = "a";
    protected List<SkillSO> all_skills = new List<SkillSO>();
    protected List<SkillSO> skill_equipped = new List<SkillSO>();
    public IReadOnlyList<SkillSO> AllSkills => all_skills;
    public IReadOnlyList<SkillSO> SkillEquipped => skill_equipped;

    public void EquipSkill(SkillSO data)
    {
        if (skill_equipped.Contains(data) || !all_skills.Contains(data) || skill_equipped.Count >= 4) return;
        skill_equipped.Add(data);
    }
    public void UnequipSkill(SkillSO data)
    {
        if (!skill_equipped.Contains(data)) return;
        skill_equipped.Remove(data);
    }

    public PJData(PJSO so) : base(so)
    {
        
        this.all_skills = so.skills;
        this.skill_equipped = so.skills;
    }
}
