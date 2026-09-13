using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class BattleContext
{
    public BattleChar Self;
    public List<BattleChar> Target = new List<BattleChar>();
    public List<BattleChar> Party = new List<BattleChar>();
    public List<BattleChar> Enemies = new List<BattleChar>();
    public int SelfPosition;
}
