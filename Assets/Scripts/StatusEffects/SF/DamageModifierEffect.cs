using UnityEngine;

public class DamageModifierEffect : StatusEffectData
{
    private readonly int value;

    public DamageModifierEffect(BattleChar source, BattleChar owner, StatusEffectSO so, int stacks,
        int value, bool resist, int duration = -1)
        : base(source, owner, 1, so, resist, duration >= 0 ? duration : stacks, 1)
    {
        this.value = value;
    }

    public override void OnDamageDealt(DamageInfo info)
    {
        if (info == null) return;
        info.damage += (info.damage * value) / 100;
        TickDuration();
    }
}