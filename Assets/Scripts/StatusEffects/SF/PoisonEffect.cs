using UnityEngine;

[System.Serializable]
public class PoisonEffect : StatusEffectData
{
    public int tier;
    public PoisonEffect(BattleChar source, BattleChar owner, int stacks, StatusEffectSO so, int tier, bool resist) : base(source, owner, stacks, so, resist)
    {
        this.tier = tier;
    }

    public override void OnEndTurn()
    {
        DamageInfo info = new DamageInfo(source, owner, stacks * tier, true);
        owner.TakeDamage(info);
        stacks--;
    }
}
