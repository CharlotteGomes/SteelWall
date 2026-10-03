using UnityEngine;

public class Penitence : StatusEffectData
{
    public Penitence(BattleChar source, BattleChar owner, int stacks, StatusEffectSO so, bool resist) : base(source, owner, stacks, so, resist)
    {

    }

    public override void OnDamageDealt(DamageInfo info)
    {
        info.damage -= (info.damage * (10 * stacks)) / 100;
    }
}
