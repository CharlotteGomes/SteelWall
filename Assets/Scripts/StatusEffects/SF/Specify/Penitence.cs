using UnityEngine;
using System.Collections.Generic;
public class Penitence : StatusEffectData
{
    public StatsModifierSO[] original_value;
    public StatsModifierSO[] modifiers; 
    public Penitence(BattleChar source, BattleChar owner, int stacks, StatusEffectSO so, StatsModifierSO[] mods, bool resist = false) : base(source, owner, stacks, so, resist)
    {
        original_value = mods;
        List<StatsModifierSO> temp = new List<StatsModifierSO>();
        foreach(var m in mods)
        {
            temp.Add(ScriptableObject.Instantiate(m));
        }
        modifiers = temp.ToArray();
    }

    public override void OnApply()
    {
        foreach(var modifier in modifiers)
        {
            owner.ReceiveStatsModifier(modifier);
        }
    }

    public override void OnStackPile(StatusEffectData status)
    {
        if (stacks >= 3) return;

        base.OnStackPile(status);
        if (status is not Penitence p) return;
        for (int i = 0; i < modifiers.Length; i++) 
        {
            modifiers[i].value += original_value[i].value;
        }
    }

    public override void OnRemove()
    {
        foreach (var m in modifiers)
        {
            owner.RemoveStatsModifier(m);
        }
        
    }
}
