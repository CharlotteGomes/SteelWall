using UnityEngine;

[System.Serializable]
public class StatusEffectData
{
    public string effectName;
    public StatusEffectSO sfSO;

    // Number of accumulated applications/stacks; independent from effect strength and lifetime.
    public int stacks;
    public int potency;
    // -1 means the status has no timed expiration.
    public int remainingDuration = -1;

    public BattleChar owner;
    public BattleChar source;
    public bool resistiable;

    public StatusEffectData(BattleChar source, BattleChar owner, int stacks, StatusEffectSO so, bool resist)
        : this(source, owner, stacks, stacks, -1, so, resist)
    {
    }

    public StatusEffectData(BattleChar source, BattleChar owner, int stacks, int potency, int duration, StatusEffectSO so, bool resist)
    {
        this.owner = owner;
        this.source = source;
        this.stacks = Mathf.Max(0, stacks);
        this.potency = Mathf.Max(0, potency);
        remainingDuration = duration < 0 ? -1 : duration;
        effectName = so != null ? so.effectName : string.Empty;
        resistiable = resist;
        sfSO = so;
    }

    public bool HasExpired => remainingDuration == 0;

    protected void AdvanceDuration()
    {
        if (remainingDuration > 0)
            remainingDuration--;
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
        if (status == null) return;
        stacks += status.stacks;
        potency += status.potency;

        if (remainingDuration < 0 || status.remainingDuration < 0)
            remainingDuration = -1;
        else
            remainingDuration = Mathf.Max(remainingDuration, status.remainingDuration);
    }

    public virtual void OnCharacterDeath() { }
}
