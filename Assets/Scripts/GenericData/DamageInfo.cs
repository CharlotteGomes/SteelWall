using UnityEngine;

[System.Serializable]
public class DamageInfo
{
    public BattleChar dealer { get; private set; }
    public BattleChar receiver { get; private set; }
    public int damage;
    public bool ignore_prot = false;
    public DamageInfo(BattleChar dealer, BattleChar receiver, int damage, bool ignore_prot = false)
    {
        this.dealer = dealer;
        this.receiver = receiver;
        this.damage = damage;
        this.ignore_prot = ignore_prot;
    }
}