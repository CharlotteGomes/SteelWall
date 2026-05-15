using NUnit.Framework;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public abstract class BattleCharacter : Character
{
    public Action<BattleCharacter> OnDeath;
    public Action<BattleCharacter> OnTurnStart;
    public Action<BattleCharacter> OnTurnEnd;
    public BattleContext ctx;
    public List<SkillBase> SkillList = new List<SkillBase>();

    public override void InitializeStats()
    {
        if (stats is BattleStats bStats) { SkillList = bStats.skills; }
        base.InitializeStats();
    }
    public virtual void TakeDamage(int damage)
    {
        if (damage >= 0) { LifeCurrent -= damage; }
        if (LifeCurrent < 0) { }
    }

    public virtual void Die()
    {
        LifeCurrent = 0;
        OnDeath?.Invoke(this);
    }

    public abstract IEnumerator ExecuteTurn();
}
