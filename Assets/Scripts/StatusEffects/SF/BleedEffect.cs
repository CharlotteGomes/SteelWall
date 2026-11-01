using UnityEngine;

public class BleedEffect : StatusEffectData
{
    public BleedEffect(BattleChar source, BattleChar owner, int stacks, StatusEffectSO so, bool resist,
        int duration = -1, int potency = -1)
        : base(source, owner, 1, so, resist,
            duration >= 0 ? duration : stacks,
            potency >= 0 ? potency : stacks)
    {
    }

    public override void OnStartTurn()
    {
        if (owner == null || !owner.CheckAlive()) return;
        DamageInfo info = new DamageInfo(source, owner, potency, true);
        owner.TakeDamage(info);
        TickDuration();
    }
}