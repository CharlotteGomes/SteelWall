using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "BattleData", menuName = "Scriptable Objects/BattleData")]
public class BattleData : ScriptableObject
{
    public List<EnemyStats> EnemiesData;
}
