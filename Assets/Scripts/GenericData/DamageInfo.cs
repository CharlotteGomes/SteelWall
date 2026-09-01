using UnityEngine;

public class DamageInfo
{
    public BattleChar dealer { get; private set; }
    public BattleChar receiver { get; private set; }
    public float damage { get; private set; }
    public DamageType damageType { get; set; }

    public DamageInfo(BattleChar dealer, BattleChar receiver, float damage, DamageType damageType)
    {
        this.dealer = dealer;
        this.receiver = receiver;
        this.damage = damage;
        this.damageType = damageType;
    }

    public void ModifyDamage(float amount)
    {
        damage += amount;
    }
}