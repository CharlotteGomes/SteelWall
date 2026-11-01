using UnityEngine;

public class MarkEffect : StatusEffectData
{
    public MarkEffect(BattleChar source, BattleChar owner, StatusEffectSO so, int stacks, bool resist,
        int duration = -1, int potency = -1)
        : base(source, owner, stacks, so, resist, duration, potency)
    {
    }
}