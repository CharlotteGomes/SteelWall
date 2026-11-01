using UnityEngine;

[System.Serializable]
public class StatusEffectData
{
    public string effectName;
    public StatusEffectSO sfSO;
    public int stacks;
    public BattleChar owner;
    public BattleChar source;
    public bool resistiable;
    public StatusEffectData(BattleChar source, BattleChar owner, int stacks, StatusEffectSO so, bool resist)
    {
        this.owner = owner;
        this.source = source;
        this.stacks = stacks;
        this.effectName = so.effectName;
        resistiable = resist;
        sfSO = so;
    }

    public virtual void OnApply() { }
    public virtual void OnRemove() { }
    public virtual void OnStartTurn() { }
    public virtual void OnEndTurn() { }
    public virtual void OnRoundEnd() { }
    public virtual void OnUseSkill(BattleResult result) { }
    public virtual void OnDamageDealt(DamageInfo info) { }
    public virtual void OnDamageTaken(DamageInfo info) { }
    public virtual void OnHealGiven(HealInfo info) { }
    public virtual void OnHealReceived(HealInfo info) { }
    public virtual void OnGiveStatus(StatusEffectData status) { }
    public virtual void OnReceiveStatus(StatusEffectData status) { }  
    public virtual void OnStatusRemoved(StatusEffectData status) { }

    public virtual void OnStackPile(StatusEffectData status)
    {
        stacks += status.stacks;
    }

    public virtual void OnCharacterDeath() { }
}
