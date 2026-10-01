using UnityEngine;

public class StatModifierEffect : StatusEffectData
{
    StatsModifierSO modifier;
    public StatModifierEffect(BattleChar source, BattleChar owner, StatusEffectSO so, StatsModifierSO modifier, string efName) : base(source, owner, so, efName)
    {
        this.modifier = modifier;
    }

    public override void OnApply()
    {
        owner.ReceiveStatsModifier(modifier);
    }

    public override void OnEndTurn()
    {
        stacks--;
        if (stacks <= 0)
        {
           owner.RemoveStatsModifier(modifier);
           owner.StatusRemove(this);
        }
    }
}
