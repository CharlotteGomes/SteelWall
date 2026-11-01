using UnityEngine;

public abstract class ApplyStatusEffect : SkillEffectSO
{
    public StatusEffectSO sfSO;
    public int stacks = 1;
    public bool resist;
    [Min(-1)] public int duration = -1;
    [Min(-1)] public int potency = -1;

    protected int RuntimeDuration => duration >= 0 ? duration : stacks;
    protected int RuntimePotency => potency >= 0 ? potency : stacks;
}