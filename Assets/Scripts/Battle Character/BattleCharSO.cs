using UnityEngine;
using System.Collections.Generic;
using System.Drawing;

public abstract class BattleCharSO : ScriptableObject
{
    public string Name;
    public CharacterSpriteSO sprites;
    public int life, protect, dodge, damage_min, damage_max, accuracy, spd;
    public int move_range = 1;
    public int stun, debuff, bleed, poison, move, sanity;
    public int crit = 5;
    public int size = 1;
}
