using UnityEngine;

[System.Serializable]
public class StatusEffectData
{
    public string effectName;
    public StatusEffectSO sfSO;
    public int stacks;
    public int timeRemaining;
    public BattleChar owner;
    public BattleChar source;
    public StatusEffectData(BattleChar source, BattleChar owner, StatusEffectSO so)
    {
        this.owner = owner;
        this.source = source;
        this.timeRemaining = so.duration;
        this.stacks = so.stacks;
        this.effectName = so.effectName;    
        sfSO = so;
    }

    public virtual void OnApply() { }
    public virtual void OnRemove() { }
    public virtual void OnStartTurn() { }
    public virtual void OnEndTurn() { }
    public virtual void OnDamageDealt(DamageInfo info) { }
    public virtual void OnDamageTaken(DamageInfo info) { }
    public virtual void OnHealGiven(HealInfo info) { }
    public virtual void OnHealReceived(HealInfo info) { }
    public virtual void OnGiveStatus(StatusEffectSO status) { }
    public virtual void OnReceiveStatus(StatusEffectSO status) { }  
    public virtual void OnStatusRemoved(StatusEffectSO status) { }

    public virtual void OnCharacterDeath() { }
}
