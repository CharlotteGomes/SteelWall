using System.Collections;
using UnityEngine;

public enum TargetType
{
    Allies,
    Enemies
}

public abstract class SkillBase : ScriptableObject
{
    public string Name;
    public string Description;
    public Sprite Icon;
    public int Cooldown;
    public int ManaCost;
    public string Animation;

    public TargetType tTarget;
    public abstract IEnumerator Execute(BattleContext ctx);
}

