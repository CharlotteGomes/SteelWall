using System.Data.Common;
using UnityEngine;

public abstract class BaseStats: ScriptableObject
{
    public string Name;
    public int strength, dexterity, vigor, inteligence, knowledge, faith;
}

[CreateAssetMenu(menuName = "Scriptable Objects/Stats/PJStats")]
public class PJStats : BaseStats
{

}

[CreateAssetMenu(menuName = "Scriptable Objects/Stats/EnemyStats")]
public class EnemyStats : BaseStats
{

}