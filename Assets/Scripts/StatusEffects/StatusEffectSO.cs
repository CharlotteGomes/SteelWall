using UnityEngine;

[System.Serializable]
public class StatusEffectSO : ScriptableObject
{
    public string effectName;
    public Sprite icon;
    public int stacks;
    public int duration;

    public virtual void OnApply() { }
    public virtual void OnRemove() { }
    public virtual void OnStartTurn() { }
    public virtual void OnEndTurn() { }
    public virtual void OnDamageDealt(DamageInfo info) { }
    public virtual void OnDamageTaken(DamageInfo info) { }
    public virtual void OnHealGiven(HealInfo info) { }
    public virtual void OnHealReceived(HealInfo info) { }
    public virtual void OnStatusApplied(StatusEffectSO status) { }
    public virtual void OnStatusRemoved(StatusEffectSO status) { }
    

}
