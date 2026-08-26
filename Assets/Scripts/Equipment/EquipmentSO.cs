using UnityEngine;

public enum EquipmentType
{
    Weapon,
    Armor
}

public enum ArmorType
{
    Helmet,
    Chestplate,
    Bracelet,
    Gloves,
    Legs,
    Greaves,
}

public enum WeaponType
{
    Any,
    ShortSword,
    Dagger,
    BattleAxe,
    Mace
}
public abstract class EquipmentSO : ScriptableObject
{
    public EquipmentType e_type;
    public Sprite icon;
    public string Name;
    public BonusStatsSO[] bonus_stats;
    public PassiveSO[] passives;
    public virtual void Apply(PjController pj)
    {
        foreach(PassiveSO p in passives)
        {
            p.Apply(pj);
        }

        foreach (BonusStatsSO s in bonus_stats)
        {
            pj.AddStat(s.stat, s.bonus);
        }
    }
    public void Remove(PjController pj) 
    {
    
    }
}
