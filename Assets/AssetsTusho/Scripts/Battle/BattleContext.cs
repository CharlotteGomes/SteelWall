using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class BattleContext
{
    public BattleCharacter user;
    public BattleCharacter target;
    public List<BattleCharacter> allies = new List<BattleCharacter>();
    public List<BattleCharacter> enemies = new List<BattleCharacter>();

    public BattleContext(BattleCharacter user, List<BattleCharacter> allies, List<BattleCharacter> enemies)
    {
        this.user = user;
        this.allies = allies;
        this.enemies = enemies;
    }
}
