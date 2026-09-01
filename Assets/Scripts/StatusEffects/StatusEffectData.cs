using UnityEngine;

[System.Serializable]
public class StatusEffectData
{
    public StatusEffectSO st_effect { get; protected set; }
    public BattleChar owner { get; protected set; }
    public int stacks { get; protected set; }
    public int time_remaining { get; protected set; }

    public StatusEffectData(BattleChar owner)
    {
        this.owner = owner;
    }

    public virtual void OnApply() { }
    public virtual void OnRemove() { }
    public virtual void OnStartTurn() { }
    public virtual void OnEndTurn() { }
    public virtual void OnDamageDealt(DamageInfo info) { }
    public virtual void OnDamageTaken(DamageInfo info) { }
    public virtual void OnHealGiven(HealInfo info) { }
    public virtual void OnHealReceived(HealInfo info) { }
    public virtual void OnStatusApplied(StatusEffectData status) { }
    public virtual void OnStatusRemoved(StatusEffectData status) { }
    

}
