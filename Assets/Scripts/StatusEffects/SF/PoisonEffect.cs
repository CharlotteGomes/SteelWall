using UnityEngine;

[System.Serializable]
public class PoisonEffect : StatusEffectData
{
    public int tier;

    public PoisonEffect(BattleChar source, BattleChar owner, int stacks, StatusEffectSO so, int tier, bool resist,
        int duration = -1, int potency = -1)
        : base(source, owner, 1, so, resist,
            duration >= 0 ? duration : stacks,
            potency >= 0 ? potency : stacks)
    {
        this.tier = tier;
    }

    public override string GetRuntimeDescription()
    {
        int damage = potency * tier;
        return damage > 0
            ? $"Causa {damage} de dano no final do turno do personagem."
            : "Causa dano no final do turno do personagem.";
    }

    public override void OnEndTurn()
    {
        if (owner == null || !owner.CheckAlive()) return;
        DamageInfo info = new DamageInfo(source, owner, potency * tier, true);
        owner.TakeDamage(info);
        TickDuration();
    }
}