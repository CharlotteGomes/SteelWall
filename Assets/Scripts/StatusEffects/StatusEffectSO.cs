using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "StatusEffectBase", menuName = "Scriptable Objects/StatusEffect/StatusEffectBase")]
public class StatusEffectSO : ScriptableObject
{
    public ResistenceType type;
    public string effectName;
    public int stacks;
    public Sprite icon;
    public bool resistable;
}
