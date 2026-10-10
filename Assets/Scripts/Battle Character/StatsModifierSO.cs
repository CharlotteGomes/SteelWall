using UnityEngine;

[CreateAssetMenu(fileName = "StatsModifierSO", menuName = "Scriptable Objects/StatsModifierSO")]
public class StatsModifierSO : ScriptableObject
{
    public StatType statType;
    public int value;
    public bool flat;
}
