using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;

public class BattleResult
{
    public List<BattleChar> dodge = new List<BattleChar>();
    public List<BattleChar> hit = new List<BattleChar>();
    public int acc_base;
    public int acc_roll;
    public bool skill_fail = true;
    public bool crit = false;
  
}
