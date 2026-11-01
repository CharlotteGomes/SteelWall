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

    public override void OnApply()
    {
        if (owner == null || modifiers == null) return;
        foreach (var modifier in modifiers)
            owner.ReceiveStatsModifier(modifier);
    }

    public override void OnRoundEnd()
    {
        TickDuration();
    }

    public override void OnStackPile(StatusEffectData status)
    {
        if (status == null || stacks >= 3) return;
        int previousStacks = stacks;
        int addedStacks = Mathf.Min(status.stacks, 3 - previousStacks);
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

        if (modifiers != null)
        {
            foreach (var modifier in modifiers)
                if (modifier != null) Object.Destroy(modifier);
        }
    }
}