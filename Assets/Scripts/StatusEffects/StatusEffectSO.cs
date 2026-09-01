using UnityEngine;

[System.Serializable]
public class StatusEffectSO : ScriptableObject
{
    public string effectName;
    public Sprite icon;
    public int stacks;
    public int duration;
}
