using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class FavoritePos
{
    public int value = 0;
    public List<SkillSO> skills = new List<SkillSO>();    
    public List<BattleChar> targets = new List<BattleChar>();

    public void AddSkill(SkillSO so)
    {
        skills.Add(so);
    }

    public void AddTarget(BattleChar target)
    {
        targets.Add(target);
    }

    public void SetValue()
    {
        value = targets.Count;
    }
}
