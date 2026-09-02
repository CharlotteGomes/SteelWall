using UnityEngine;

public class HealInfo
{
    public BattleChar healer;
    public float heal_amount;
    public BattleChar target;
    public HealInfo(BattleChar healer, float heal_amount, BattleChar target)
    {
        this.healer = healer;
        this.heal_amount = heal_amount;
        this.target = target;
    }
}
