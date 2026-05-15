using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public abstract class BaseStats: ScriptableObject
{
    public string Name;
    public int strength, dexterity, vigor, inteligence, knowledge, faith;
}

public abstract class BattleStats : BaseStats
{
    public List<SkillBase> skills = new List<SkillBase>();
}

