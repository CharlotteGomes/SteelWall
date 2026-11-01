using UnityEngine;


[System.Serializable]
public class PoisonEffect : StatusEffectData
{
    public int tier;

    public PoisonEffect(BattleChar source, BattleChar owner, int stacks, StatusEffectSO so, int tier, bool resist)
        : this(source, owner, stacks, stacks, stacks, so, tier, resist) { }

    public PoisonEffect(BattleChar source, BattleChar owner, int stacks, int potency, int duration, StatusEffectSO so, int tier, bool resist)
        : base(source, owner, stacks, potency, duration, so, resist)
    {
        this.tier = tier;
    }

    public override void OnEndTurn()
    {
        if (owner == null || HasExpired) return;
        DamageInfo info = new DamageInfo(source, owner, potency * tier, true);
        owner.TakeDamage(info);
        AdvanceDuration();
    }
}
