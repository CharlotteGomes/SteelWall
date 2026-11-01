using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public abstract class BattleCharData
{
    public string Name { get; private set; }
    public CharacterSpriteSO sprites { get; private set; }
    public int life { get; private set; }

    public int protect { get; private set; }
    public int dodge { get; private set; }
    public int damage_min { get; private set; }
    public int damage_max { get; private set; }
    public int accuracy { get; private set; }
    public int spd { get; private set; }

    public int move_range { get; private set; } = 1;
    public int stun { get; private set; }
    public int debuff{ get; private set; }
    public int bleed { get; private set; }
    public int poison { get; private set; }
    public int move { get; private set; }
    public int sanity { get; private set; }
    public int crit { get; private set; } = 5;

    public BattleCharData(BattleCharSO so)
    {
        Name = so.Name;
        sprites = so.sprites;

        life = so.life;
        protect = so.protect;
        dodge = so.dodge;
        damage_min = so.damage_min;
        damage_max = so.damage_max;
        accuracy = so.accuracy;
        spd = so.spd;
        move_range = so.move_range;

        stun = so.stun;
        debuff = so.debuff;
        bleed = so.bleed;
        poison = so.poison;
        move = so.move;
        sanity = so.sanity;
        crit = so.crit;
        
            

    }
}
