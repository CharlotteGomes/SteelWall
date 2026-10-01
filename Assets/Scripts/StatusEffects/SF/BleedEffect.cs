using UnityEngine;

public class BleedEffect : StatusEffectData
{
    public BleedEffect(BattleChar source, BattleChar owner, StatusEffectSO so, string efName) : base(source, owner, so, efName)
    {
    }
    public override void OnStartTurn()
    {
        DamageInfo info = new DamageInfo(source, owner, stacks);
        owner.TakeDamage(info);
        stacks--;
        if (stacks <= 0) owner.StatusRemove(this);
    }
}
