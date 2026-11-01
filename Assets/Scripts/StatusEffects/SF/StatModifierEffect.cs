using UnityEngine;

public class StatModifierEffect : StatusEffectData
{
    private readonly StatsModifierSO modifier;

    public StatModifierEffect(BattleChar source, BattleChar owner, int stacks, StatusEffectSO so,
        StatsModifierSO modifier, bool resist, int duration = -1)
        : base(source, owner, 1, so, resist, duration >= 0 ? duration : stacks, 1)
    {
        this.modifier = modifier;
    }

    public override void OnApply()
    {
        if (owner != null && modifier != null)
            owner.ReceiveStatsModifier(modifier);
    }

    public override void OnRoundEnd()
    {
        TickDuration();
    }

    public override void OnRemove()
    {
        if (owner != null && modifier != null)
            owner.RemoveStatsModifier(modifier);
    }
}