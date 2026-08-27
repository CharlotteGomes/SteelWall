using UnityEngine;

public abstract class ConditionalSkillSO : ScriptableObject
{
    public int amount_uses;
    public SkillSO skill;
    public abstract bool Check();
}
