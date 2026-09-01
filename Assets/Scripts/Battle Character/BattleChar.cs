using System.Reflection;
using UnityEngine;
using System.Collections;
using System;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEditor.Playables;
using UnityEditor.UI;
using Unity.VisualScripting;

/*public enum BattleOptions 
{ 
    Attack = 0,
    Skill = 1,
}*/
public abstract class BattleChar : MonoBehaviour
{
    [Header("BattleInfo")]
    [SerializeField] protected BattleCharSO battle_char;
    [SerializeField] protected BattleContext ctx;
    [SerializeField] protected SkillSO skill_current;
    [SerializeField] protected TurnStep step;
    public CharacterSpriteSO spriteSO { get; private set; }
    
    protected int strength, dexterity, constitution, intelligence, mind;
    [SerializeField] protected int life_max, life_current;
    protected int r_slashing, r_bludgeoning, r_pierce, r_fire, r_ice, r_lightning, r_purity;

    [Header("Ref")]
    [SerializeField] Animator animator;

    [Header("Actions")]
    public Action<BattleChar> Death;
    public Action<BattleChar> start_turn;
    public Action<BattleChar> end_turn;
    public Action<BattleChar> deal_damage;
    public Action<BattleChar> take_damage;
    public Action<BattleChar> deal_heal;
    public Action<BattleChar> receive_heal;
    public Action<BattleChar> apply_status_effect;
    public Action<BattleChar> receive_status_effect;

    #region Stats
    public int Resistences(TypeDmg dmg)
    {
        switch (dmg) 
        {
            case TypeDmg.Slashing:
                return r_slashing;
            case TypeDmg.Bludgeoning:
                return r_bludgeoning;
            case TypeDmg.Pierce:
                return r_pierce;
            case TypeDmg.Fire:
                return r_fire;
            case TypeDmg.Ice:
                return r_ice;
            case TypeDmg.Lightning:
                return r_lightning;
            case TypeDmg.Purity:
                return r_purity;
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
    public void AddStat(Stats stats, int value)
    {
        if (value < 0) return;
        switch (stats)
        {
            case Stats.Strength:
                strength += value; break;
            case Stats.Dexterity:
                dexterity += value; break;
            case Stats.Constitution:
                constitution += value; break;
            case Stats.Int:
                intelligence += value; break;
            case Stats.Mind:
                mind += value; break;
            case Stats.Slashing:
                r_slashing += value; break;
            case Stats.Bludgeoning:
                r_bludgeoning += value; break;
            case Stats.Pierce:
                r_pierce += value; break;
            case Stats.Fire:
                r_fire += value; break;
            case Stats.Ice:
                r_ice += value; break;
            case Stats.Lightning:
                r_lightning += value; break;
            case Stats.Purity:
                r_purity += value; break;
        }
    }
    public void RemoveStat(Stats stats, int value)
    {
        if (value < 0) return;
        switch (stats)
        {
            case Stats.Strength:
                strength -= value; break;
            case Stats.Dexterity:
                dexterity -= value; break;
            case Stats.Constitution:
                constitution -= value; break;
            case Stats.Int:
                intelligence -= value; break;
            case Stats.Mind:
                mind -= value; break;
            case Stats.Slashing:
                r_slashing -= value; break;
            case Stats.Bludgeoning:
                r_bludgeoning -= value; break;
            case Stats.Pierce:
                r_pierce -= value; break;
            case Stats.Fire:
                r_fire -= value; break;
            case Stats.Ice:
                r_ice -= value; break;
            case Stats.Lightning:
                r_lightning -= value; break;
            case Stats.Purity:
                r_purity -= value; break;
        }
    }
    public void SetBattleChar(BattleCharSO battle_char)
    {
        this.battle_char = battle_char; 
    }

    #endregion

    #region Init
    public virtual void InitStats()
    {
        spriteSO = battle_char.sprites;
        strength = battle_char.Str;
        dexterity = battle_char.Dex;
        constitution = battle_char.Cons;
        intelligence = battle_char.Int;
        mind = battle_char.Mnd;
    }
    #endregion

    #region Generic
    public void TakeDamage(TypeDmg type, int amount)
    {
        if(amount > 0)
        {
            float dmg = (float)amount * (1 - Resistences(type) / 100);
            if(dmg > 0)
            {
                life_current = Mathf.Clamp(life_current -= (int)dmg, 0, 1000);
            }
        }
    }
    public bool CheckAlive()
    {
        return life_current > 0 ? true : false;
    }

    public void Die()
    {
        Death?.Invoke(this);
    }
    #endregion

    #region Skill functions
    public virtual IEnumerator StartSkill()
    {
        yield return StartCoroutine(OnGoingSkill());
    }

    public virtual IEnumerator OnGoingSkill()
    {
        List<BattleChar> Targets = new();
        switch (skill_current.target)
        {
            case TypeTarget.ChooseParty:
                Targets.Add(ctx.Target);break;

            case TypeTarget.ChooseEnemy:
                Targets.Add(ctx.Target);break;

            case TypeTarget.Enemy:
                foreach(int i in skill_current.pos)
                {
                    Targets.Add(ctx.Enemies[i]);
                }break;
            case TypeTarget.Party:
                foreach (int i in skill_current.pos)
                {
                    Targets.Add(ctx.Party[i]);
                }
                break;
        }
        Debug.Log($"{ctx.Self.name} {Targets.Count} {skill_current.name}");
        StartCoroutine(GM.instance.battle_manager.battle_presentation.StartPresentation(ctx.Self, Targets, skill_current.presentation));
        yield return StartCoroutine(FinishingSkill());
    }

    public virtual IEnumerator FinishingSkill()
    {
        skill_current.Execute(ctx);
        yield return null;
    }
    #endregion

    #region Turn
    public abstract IEnumerator OnGoingTurn();
    #endregion
    public void CreateContext(List<BattleChar> allies, List<BattleChar> enemies)
    {
        ctx = new BattleContext();
        ctx.Self = this;
        ctx.Party = allies;
        ctx.Enemies = enemies;
    }
}
