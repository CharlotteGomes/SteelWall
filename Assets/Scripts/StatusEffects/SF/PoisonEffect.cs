using UnityEngine;


public class PoisonEffect : StatusEffectData
{
    public int tier;
    public PoisonEffect(BattleChar source, BattleChar owner, StatusEffectSO so, int tier) : base(source, owner, so)
    {
        this.tier = tier;
    }

    public override void OnStartTurn()
    {
        Poison type = ScriptableObject.CreateInstance<Poison>();
        DamageInfo info = new DamageInfo(source, owner, stacks * tier, type);
        Debug.Log("OOOOOOOOOLHA O VENENOOOOOOO");
        owner.TakeDamage(info);
    }
}
