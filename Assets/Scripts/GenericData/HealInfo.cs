using UnityEngine;

public class HealInfo
{
    public BattleChar healer { get; private set; }
    public float heal_amount { get; private set; }
    public HealInfo(BattleChar healer, float heal_amount)
    {
        this.healer = healer;
        this.heal_amount = heal_amount;
    }
}
