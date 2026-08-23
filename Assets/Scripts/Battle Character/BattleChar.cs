using System.Reflection;
using UnityEngine;
using System.Collections;
using System;
using NUnit.Framework;
using System.Collections.Generic;

public abstract class BattleChar : MonoBehaviour
{
    [Header("BattleInfo")]
    [SerializeField] protected BattleCharSO battle_char;
    [SerializeField] protected BattleContext ctx;
    [SerializeField] protected SkillSO[] skills;
    [SerializeField] protected SkillSO skill_current;
    [SerializeField] protected bool is_my_turn = false;

    [SerializeField] protected int strength, dexterity, constitution, intelligence, mind;
    [SerializeField] protected int life_max, life_current, mana_max, mana_current;

    [Header("Ref")]
    [SerializeField] Animator animator;

    public Action<BattleChar> Death;
    public Action<BattleChar> StartTurn;
    public Action<BattleChar> EndTurn;
    public int Resistences(TypeDmg dmg)
    {
        switch (dmg) 
        {
            case TypeDmg.Slashing:
                return 50;
            case TypeDmg.Bludgeoning:
                return 50;
            case TypeDmg.Pierce:
                return 50;
            case TypeDmg.Fire:
                return 50;
            case TypeDmg.Ice:
                return 50;
            case TypeDmg.Lightning:
                return 50;
            case TypeDmg.Purity:
                return 50;
            default:
                return 0;
        }
    }//Function for make something stats more easy to access
    public int ReadStats(Stats stats)
    {
        switch (stats) 
        {
            case Stats.Strength:
                return strength;
            case Stats.Dexterity:
                return dexterity;
            case Stats.Constitution:
                return constitution;
            case Stats.Mind:
                return mind;
            case Stats.Int:
                return intelligence;
            default:
                return 0;

        }
    }//Function for make something stats more easy to access
    public void SetBattleChar(BattleCharSO battle_char)
    {
        this.battle_char = battle_char; 
    }
    public virtual void InitStats()
    {
        strength = battle_char.Str;
        dexterity = battle_char.Dex;
        constitution = battle_char.Cons;
        intelligence = battle_char.Int;
        mind = battle_char.Mnd;
    }

    public void TakeDamage(TypeDmg type, int amount)
    {
        if(amount > 0)
        {
            float dmg = (float)amount * (1 - (float)Resistences(type) / 100);
            if(dmg > 0)
            {
                Debug.Log($"Tomou {dmg} de dano");
                life_current = Mathf.Clamp(life_current -= (int)dmg, 0, 1000);
            }
        }
    }
    public bool CheckAlive()
    {
        return life_current >= 0 ? true : false;
    }

    public void Die()
    {
        Death?.Invoke(this);
    }

    public virtual IEnumerator StartSkill()
    {
        if (skill_current == null) yield break;
        //animator.Play(skill_current.clip.name);
    }

    public virtual IEnumerator OnGoingSkill()
    {
        yield return null;
    }

    public virtual void ExecuteSkillsEffects()
    {
        if (skill_current == null) return;
        foreach(SkillEffectSO f in skill_current.effects)
        {
            f.Execute(ctx);
        }
    }

    public void CreateContext(List<BattleChar> allies, List<BattleChar> enemies)
    {
        ctx = new BattleContext();
        ctx.Self = this;
        ctx.Party = allies;
        ctx.Enemies = enemies;
    }
}
