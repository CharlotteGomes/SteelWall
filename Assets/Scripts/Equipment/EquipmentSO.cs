using UnityEngine;

public abstract class EquipmentSO : ScriptableObject
{
    public Sprite icon;
    public string Name;
    public BonusStatsSO[] bonus_stats;
    public PassiveSO[] passives;
    public void Apply(PjController pj)
    {
        foreach(PassiveSO p in passives)
        {
            p.Apply(pj);
        }

        foreach (BonusStatsSO s in bonus_stats)
        {
            
        }
    }
    public abstract void Remove(PjController pj);
}
