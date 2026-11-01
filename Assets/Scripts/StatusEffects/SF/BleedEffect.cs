using UnityEngine;


public class BleedEffect : StatusEffectData
{
    public BleedEffect(BattleChar source, BattleChar owner, int stacks, StatusEffectSO so, bool resist)
        : this(source, owner, stacks, stacks, stacks, so, resist) { }

    public BleedEffect(BattleChar source, BattleChar owner, int stacks, int potency, int duration, StatusEffectSO so, bool resist)
        : base(source, owner, stacks, potency, duration, so, resist) { }

    public override void OnStartTurn()
    {
        if (owner == null || HasExpired) return;
        DamageInfo info = new DamageInfo(source, owner, potency, true);
        owner.TakeDamage(info);
        AdvanceDuration();
    }
}
