using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "StatusEffect", menuName = "Scriptable Objects/StatusEffect")]
public class StatusEffectSO : ScriptableObject
{
    public ResistenceType type;
    public string effectName;
    public Sprite icon;
}
