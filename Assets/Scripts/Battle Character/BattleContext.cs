using UnityEngine;
using System.Collections.Generic;

public enum TypeTarget
{
    Self,
    OneTarget,
    AdjacentTarget,
    AllEnemy,
    OneParty,
    AllParty,
}

[System.Serializable]
public class BattleContext
{
    public BattleChar Self, Target;
    public List<BattleChar> Party;
    public List<BattleChar> Enemies;
}
