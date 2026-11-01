using System;

/// <summary>Small typed event seam between combat rules and optional presentation components.</summary>
public static class BattleCombatEvents
{
    public static event Action<BattleChar, SkillSO> SkillStarted;
    public static event Action<BattleChar, SkillSO, TargetResolution> TargetResolved;
    public static event Action<BattleChar, SkillSO, BattleResult> SkillCompleted;

    public static void RaiseSkillStarted(BattleChar actor, SkillSO skill) => SkillStarted?.Invoke(actor, skill);
    public static void RaiseTargetResolved(BattleChar actor, SkillSO skill, TargetResolution result) => TargetResolved?.Invoke(actor, skill, result);
    public static void RaiseSkillCompleted(BattleChar actor, SkillSO skill, BattleResult result) => SkillCompleted?.Invoke(actor, skill, result);
}
