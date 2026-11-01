using UnityEngine;

/// <summary>
/// Runtime state for one applied status. StatusEffectSO remains the shared definition;
/// this object owns mutable combat state for a single target.
/// </summary>
[System.Serializable]
public class StatusEffectData
{
    public string effectName;
    public StatusEffectSO sfSO;
    public int stacks;
    public int potency;
    public int remainingDuration;
    public BattleChar owner;
    public BattleChar source;
    public bool resistiable;

    public bool HasFiniteDuration => remainingDuration >= 0;
    public bool IsExpired => stacks <= 0 || remainingDuration == 0;

    /// <summary>Short, runtime-accurate explanation shown by the combat HUD.</summary>
    public virtual string GetRuntimeDescription()
    {
        return sfSO != null ? sfSO.description : string.Empty;
    }

    public StatusEffectData(
        BattleChar source,
        BattleChar owner,
        int stacks,
        StatusEffectSO so,
        bool resist,
        int duration = -1,
        int potency = -1)
    {
        this.owner = owner;
        this.source = source;
        this.stacks = Mathf.Max(0, stacks);
        this.potency = potency >= 0 ? potency : this.stacks;
        remainingDuration = duration;
        effectName = so != null ? so.effectName : string.Empty;
        resistiable = resist;
        sfSO = so;
    }

    public virtual void OnApply() { }
    public virtual void OnRemove() { }

    /// <summary>
    /// Releases resources owned by an application that was merged into an existing
    /// status and therefore never became an active instance on the character.
    /// </summary>
    public virtual void OnDiscarded() { }
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
    public virtual void OnCharacterDeath() { }

    public void TickDuration()
    {
        if (remainingDuration > 0)
            remainingDuration--;
    }

    public virtual void OnStackPile(StatusEffectData status)
    {
        if (status == null) return;
        stacks = Mathf.Max(0, stacks + status.stacks);
        potency = Mathf.Max(0, potency + status.potency);
        if (status.remainingDuration >= 0)
            remainingDuration = remainingDuration < 0
                ? status.remainingDuration
                : Mathf.Max(remainingDuration, status.remainingDuration);
    }
}