using UnityEngine;

public abstract class SkillEffectSO : ScriptableObject
{
    public bool self;
    public abstract void Execute(BattleChar user, BattleResult result);
}
