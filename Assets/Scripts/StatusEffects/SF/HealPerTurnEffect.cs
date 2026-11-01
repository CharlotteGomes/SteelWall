using UnityEngine;

public class HealPerTurnEffect : StatusEffectData
{
    public HealPerTurnEffect(BattleChar source, BattleChar owner, int stacks, StatusEffectSO so, bool resist,
        int duration = -1, int potency = -1)
        : base(source, owner, 1, so, resist,
            duration >= 0 ? duration : stacks,
            potency >= 0 ? potency : stacks)
    {
    }

    public override string GetRuntimeDescription()
    {
        return potency > 0
            ? $"Recupera {potency} de vida no início do turno do personagem."
            : "Recupera vida no início do turno do personagem.";
    }

    public override void OnStartTurn()
    {
        if (owner == null || !owner.CheckAlive()) return;
        HealInfo healInfo = new HealInfo(source, potency, owner);
        owner.DealHeal(healInfo);
        TickDuration();
    }
}