using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>Builds UI text from the same runtime effect instances used by skill execution.</summary>
public static class SkillDescriptionBuilder
{
    public static string Build(SkillSO skill, BattleChar caster = null)
    {
        if (skill == null) return string.Empty;
        StringBuilder builder = new StringBuilder();
        AppendLine(builder, skill.Description);
        if (skill.effects == null) return builder.ToString().Trim();
        foreach (SkillEffectSO effect in skill.effects)
            AppendEffect(builder, effect, caster);
        return builder.ToString().Trim();
    }

    public static string Build(SkillData runtimeSkill, BattleChar caster = null)
    {
        if (runtimeSkill == null || runtimeSkill.original_skill == null) return string.Empty;
        StringBuilder builder = new StringBuilder();
        AppendLine(builder, runtimeSkill.original_skill.Description);
        foreach (SkillEffectSO effect in runtimeSkill.effects)
            AppendEffect(builder, effect, caster);
        return builder.ToString().Trim();
    }

    private static void AppendEffect(StringBuilder builder, SkillEffectSO effect, BattleChar caster)
    {
        if (effect == null) return;
        if (effect is DamageTarget damage)
        {
            if (caster == null)
                AppendLine(builder, $"Dano: {damage.dmg:0.##}x do dano do personagem");
            else
            {
                int min = Mathf.Max(0, (int)(caster.GetStat(StatType.DMG_MIN) * damage.dmg));
                int max = Mathf.Max(min, (int)(caster.GetStat(StatType.DMG_MAX) * damage.dmg));
                AppendLine(builder, $"Dano: {min}–{max}");
            }
            return;
        }
        if (effect is HealEffectSO heal)
        {
            AppendLine(builder, heal.unique_value || heal.min_heal == heal.max_heal
                ? $"Cura: {heal.max_heal}"
                : $"Cura: {heal.min_heal}–{heal.max_heal}");
            return;
        }
        if (effect is ApplyStatusEffect status && status.sfSO != null)
        {
            string stacks = status.stacks > 0 ? $" ({status.stacks})" : string.Empty;
            AppendLine(builder, $"Aplica: {status.sfSO.effectName}{stacks}");
        }
    }

    private static void AppendLine(StringBuilder builder, string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return;
        if (builder.Length > 0) builder.AppendLine();
        builder.Append(value.Trim());
    }
}
