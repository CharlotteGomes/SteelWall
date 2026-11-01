using UnityEngine;


public class HealPerTurnEffect : StatusEffectData
{
    public HealPerTurnEffect(BattleChar source, BattleChar owner, int stacks, StatusEffectSO so, bool resist)
        : this(source, owner, stacks, stacks, stacks, so, resist) { }

    public HealPerTurnEffect(BattleChar source, BattleChar owner, int stacks, int potency, int duration, StatusEffectSO so, bool resist)
        : base(source, owner, stacks, potency, duration, so, resist) { }

    public override void OnStartTurn()
    {
        if (owner == null || HasExpired) return;
        HealInfo healInfo = new HealInfo(source, potency, owner);
        owner.DealHeal(healInfo);
        AdvanceDuration();
    }
}
