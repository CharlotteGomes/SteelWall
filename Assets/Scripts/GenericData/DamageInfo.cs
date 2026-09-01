using UnityEngine;

[System.Serializable]
public class DamageInfo
{
    public BattleChar dealer { get; private set; }
    public BattleChar receiver { get; private set; }
    public float damage;
    public DamageType damageType { get; set; }

    public DamageInfo(BattleChar dealer, BattleChar receiver, float damage, DamageType damageType)
    {
        this.dealer = dealer;
        this.receiver = receiver;
        this.damage = damage;
        this.damageType = damageType;
    }
}