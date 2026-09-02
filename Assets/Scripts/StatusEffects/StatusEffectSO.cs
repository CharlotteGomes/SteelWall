using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "StatusEffectBase", menuName = "Scriptable Objects/StatusEffect/StatusEffectBase")]
public class StatusEffectSO : ScriptableObject
{
    public string effectName;
    public Sprite icon;
    public int stacks;
    public int duration;
}
