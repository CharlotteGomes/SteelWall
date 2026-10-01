using UnityEngine;

public class HealPerTurnEffect : StatusEffectData
{
    public HealPerTurnEffect(BattleChar source, BattleChar owner, StatusEffectSO so, string efName) : base(source, owner, so, efName)
    {
    }

    public override void OnStartTurn()
    {
        HealInfo healInfo = new HealInfo(source, stacks, owner);
        owner.DealHeal(healInfo);
        stacks--;
        if (stacks <= 0)
        {
            owner.StatusRemove(this);
        }
    }
}
