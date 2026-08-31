using UnityEngine;

public enum TypeDmg
{
    Slashing,
    Pierce,
    Bludgeoning,
    Fire,
    Ice,
    Lightning,
    Purity
}

public enum Stats
{
    Strength,
    Dexterity,
    Constitution,
    Mind,
    Int,
    Slashing,
    Bludgeoning,
    Pierce,
    Fire,
    Ice,
    Lightning,
    Purity
}

public abstract class SkillEffectSO : ScriptableObject
{
    public abstract void Execute(BattleContext ctx, int[] pos, TypeTarget target);
}
