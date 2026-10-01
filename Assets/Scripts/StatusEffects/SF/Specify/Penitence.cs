using UnityEngine;

public class Penitence : StatusEffectData
{
    public Penitence(BattleChar source, BattleChar owner, StatusEffectSO so, string efName) : base(source, owner, so, efName)
    {

    }

    public override void OnDamageDealt(DamageInfo info)
    {
        info.damage -= (info.damage * (10 * stacks)) / 100;
    }
}
