using UnityEngine;

public class DamageModifierEffect : StatusEffectData
{
    int value;
    public DamageModifierEffect(BattleChar source, BattleChar owner, StatusEffectSO so, int value, string efName) : base(source, owner, so, efName)
    {
        this.value = value;
    }

    public override void OnDamageDealt(DamageInfo info)
    {
        info.damage += (info.damage * value) / 100;
        owner.StatusRemove(this);
    }
}
