using UnityEngine;

public class DamageModifierEffect : StatusEffectData
{
    int value;
    public DamageModifierEffect(BattleChar source, BattleChar owner, StatusEffectSO so, int stacks, int value, bool resist) : base(source, owner, stacks, so, resist)
    {
        this.value = value;
    }

    public override void OnDamageDealt(DamageInfo info)
    {
        info.damage += (info.damage * value) / 100;
        stacks--;
    }
}
