using UnityEngine;

[System.Serializable]
public class ResistenseData
{
    public DamageType resistence_type;
    [Range(0f, 1f)]
    public float value;
}
