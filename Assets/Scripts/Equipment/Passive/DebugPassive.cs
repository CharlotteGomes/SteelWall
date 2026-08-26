using UnityEngine;

[CreateAssetMenu(fileName = "DebugPassive", menuName = "Scriptable Objects/DebugPassive")]
public class DebugPassive : PassiveSO
{
    public override void Passive(BattleChar character)
    {
        Debug.Log("AAAAAAA");
    }
}
