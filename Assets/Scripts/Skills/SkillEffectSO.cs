using UnityEngine;

public abstract class SkillEffectSO : ScriptableObject
{
    public abstract void Execute(BattleContext ctx, int[] pos);
}
