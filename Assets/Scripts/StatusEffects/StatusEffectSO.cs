using UnityEngine;


public abstract class StatusEffectSO : ScriptableObject
{
    public string effect_name;
    public Sprite icon;

    public abstract void StatusEffect();
}
