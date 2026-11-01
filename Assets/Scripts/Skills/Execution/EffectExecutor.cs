using System.Linq;

/// <summary>
/// Executes the runtime copies of a skill's effects.
/// Effect assets are never executed or modified directly.
/// </summary>
public static class EffectExecutor
{
    public static void Execute(BattleChar user, BattleResult result, SkillData skillData)
    {
        if (user == null || result == null || skillData == null || result.skill_fail)
            return;

        foreach (SkillEffectSO effect in skillData.effects.ToArray())
        {
            if (effect != null)
                effect.Execute(user, result);
        }
    }
}