using UnityEngine;

public class StatModifierEffect : StatusEffectData
{
    StatsModifierSO modifier;
    public StatModifierEffect(BattleChar source, BattleChar owner, int stacks, StatusEffectSO so, StatsModifierSO modifier, bool resist) : base(source, owner, stacks, so, resist)
    {
        this.modifier = modifier;
    }

    public override void OnApply()
    {
        owner.ReceiveStatsModifier(modifier);
    }

    public override void OnRoundEnd()
    {
        stacks--;
    }

    public override void OnRemove()
    {
        owner.RemoveStatsModifier(modifier);
    }
}
