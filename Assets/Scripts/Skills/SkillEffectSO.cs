using UnityEngine;

public abstract class SkillEffectSO : ScriptableObject
{
    public abstract void Execute(BattleChar user, BattleResult result);
}
