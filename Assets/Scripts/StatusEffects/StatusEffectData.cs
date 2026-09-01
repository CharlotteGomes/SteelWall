using UnityEngine;


public abstract class StatusEffectData : ScriptableObject
{
    public string effectName;
    public StatusEffectSO sfSO;
    public int stacks;
    public int timeRemaining;
    public BattleChar owner;
    public StatusEffectData(BattleChar owner, int timeRemaining, int stacks, string effectName, StatusEffectSO so)
    {
        this.owner = owner;
        this.timeRemaining = timeRemaining;
        this.stacks = stacks;
        this.effectName = effectName;
        this.timeRemaining = timeRemaining;    
        sfSO = so;
    }
}
