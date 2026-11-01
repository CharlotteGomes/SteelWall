using UnityEngine;
using System.Collections.Generic;

public class Penitence : StatusEffectData
{
    public StatsModifierSO[] original_value;
    public StatsModifierSO[] modifiers;

    public Penitence(BattleChar source, BattleChar owner, int stacks, StatusEffectSO so,
        StatsModifierSO[] mods, bool resist = false, int duration = -1, int potency = -1)
        : base(source, owner, stacks, so, resist, duration, potency)
    {
        original_value = mods;
        List<StatsModifierSO> temp = new List<StatsModifierSO>();
        if (mods != null)
        {
            foreach (var modifier in mods)
                if (modifier != null) temp.Add(ScriptableObject.Instantiate(modifier));
        }
        modifiers = temp.ToArray();
    }

    public override string GetRuntimeDescription()
    {
        if (modifiers == null || modifiers.Length == 0)
            return "Aplica modificadores de atributos enquanto estiver ativo.";

        List<string> lines = new List<string>();
        foreach (StatsModifierSO modifier in modifiers)
        {
            if (modifier == null || modifier.value == 0) continue;
            string sign = modifier.value > 0 ? "+" : string.Empty;
            string unit = modifier.flat ? string.Empty : "%";
            lines.Add(modifier.statType + ": " + sign + modifier.value + unit);
        }

        return lines.Count > 0
            ? "Modifica os atributos do personagem:\n" + string.Join("\n", lines)
            : "Aplica modificadores de atributos enquanto estiver ativo.";
    }

    public override void OnApply()
    {
        if (owner == null || modifiers == null) return;
        for (int i = 0; i < modifiers.Length; i++)
        {
            if (modifiers[i] == null) continue;
            if (original_value != null && i < original_value.Length && original_value[i] != null)
                modifiers[i].value = original_value[i].value * Mathf.Max(1, stacks);
            owner.ReceiveStatsModifier(modifiers[i]);
        }
    }

    public override void OnRoundEnd()
    {
        TickDuration();
    }

    public override void OnStackPile(StatusEffectData status)
    {
        if (status == null) return;
        int previousStacks = stacks;
        int addedStacks = Mathf.Min(status.stacks, Mathf.Max(0, 3 - previousStacks));
        base.OnStackPile(status);
        stacks = previousStacks + addedStacks;

        if (status is not Penitence || addedStacks <= 0 || modifiers == null || original_value == null) return;
        for (int i = 0; i < modifiers.Length && i < original_value.Length; i++)
            if (modifiers[i] != null && original_value[i] != null)
                modifiers[i].value += original_value[i].value * addedStacks;
    }

    public override void OnRemove()
    {
        if (owner != null && modifiers != null)
        {
            foreach (var modifier in modifiers)
                if (modifier != null) owner.RemoveStatsModifier(modifier);
        }

        ReleaseModifiers();
    }

    public override void OnDiscarded()
    {
        ReleaseModifiers();
    }

    private void ReleaseModifiers()
    {
        if (modifiers == null) return;
        foreach (var modifier in modifiers)
            if (modifier != null) Object.Destroy(modifier);
        modifiers = System.Array.Empty<StatsModifierSO>();
    }
}