using UnityEngine;
using System.Collections.Generic;

public abstract class BattleCharSO : ScriptableObject
{
    public string Name;
    public CharacterSpriteSO sprites;
    public int life, protect, dodge, damage_min, damage_max, accuracy, spd;
    public int stun, debuff, bleed, poison, move, sanity;
    public int crit = 5;
}
