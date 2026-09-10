using UnityEngine;

[System.Serializable]
public class DamageInfo
{
    public BattleChar dealer { get; private set; }
    public BattleChar receiver { get; private set; }
    public int damage;

    public DamageInfo(BattleChar dealer, BattleChar receiver, int damage)
    {
        this.dealer = dealer;
        this.receiver = receiver;
        this.damage = damage;
    }
}