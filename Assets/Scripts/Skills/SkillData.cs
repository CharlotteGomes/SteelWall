using UnityEngine;

[System.Serializable]
public class SkillData
{
    public SkillSO skill;
    public int cooldown;
    public SkillData(SkillSO skill)
    {
        this.skill = skill;
        cooldown = 0;
    }
}
