using System.Collections.Generic;

/// <summary>
/// Resolves eligible targets from skill targeting rules and the current battle context.
/// This class contains no combat rolls or gameplay mutations.
/// </summary>
public static class TargetResolver
{
    public static List<BattleChar> Resolve(SkillSO skill, BattleContext context)
    {
        List<BattleChar> resolved = new List<BattleChar>();
        if (skill == null || context == null) return resolved;

        if (skill.self)
        {
            if (context.Self != null && context.Self.CheckAlive())
                resolved.Add(context.Self);
            return resolved;
        }

        List<BattleChar> candidates = skill.affect_enemy ? context.Enemies : context.Party;
        if (candidates == null || skill.pos == null) return resolved;

        foreach (BattleChar candidate in candidates)
        {
            if (candidate == null || candidate is BattleObjects || !candidate.CheckAlive())
                continue;
            if (candidate.Context == null) continue;

            int position = candidate.GetSelfPos();
            if (System.Array.IndexOf(skill.pos, position) >= 0 && !resolved.Contains(candidate))
                resolved.Add(candidate);
        }

        return resolved;
    }
}