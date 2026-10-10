using UnityEngine;

public class BleedEffect : StatusEffectData
{
    public BleedEffect(BattleChar source, BattleChar owner, int stacks, StatusEffectSO so, bool resist) : base(source, owner, stacks, so, resist)
    {
    }
    public override void OnStartTurn()
    {
        DamageInfo info = new DamageInfo(source, owner, stacks, true);
        owner.TakeDamage(info);
        Debug.Log($"Tomei {stacks} de bleed");
        stacks--;
    }
}
