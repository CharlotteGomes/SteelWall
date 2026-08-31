using UnityEngine;
using System.Collections.Generic;

public enum TypeTarget
{
    Self,
    Enemy,
    Party,
    ChooseEnemy,
    ChooseParty
}

[System.Serializable]
public class BattleContext
{
    public BattleChar Self, Target;
    public List<BattleChar> Party;
    public List<BattleChar> Enemies;
}
