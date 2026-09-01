using UnityEngine;

public abstract class DamageType : ScriptableObject
{
    public string displayName;
    public abstract void OnDamageDealt(DamageInfo info);
    public abstract void OnDamageTaken(DamageInfo info);
}
