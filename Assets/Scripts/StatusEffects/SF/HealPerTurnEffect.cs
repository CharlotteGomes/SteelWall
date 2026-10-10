using UnityEngine;

public class HealPerTurnEffect : StatusEffectData
{
    public HealPerTurnEffect(BattleChar source, BattleChar owner, int stacks, StatusEffectSO so, bool resist) : base(source, owner, stacks, so, resist)
    {
    }

    public override void OnStartTurn()
    {
        HealInfo healInfo = new HealInfo(source, stacks, owner);
        owner.DealHeal(healInfo);
        stacks--;
    }
}
