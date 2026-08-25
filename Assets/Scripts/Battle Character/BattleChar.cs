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

public abstract class BattleChar : MonoBehaviour
{
    [Header("BattleInfo")]
    [SerializeField] protected BattleCharSO battle_char;
    [SerializeField] protected BattleContext ctx;
    [SerializeField] protected SkillData skill_current;
    [SerializeField] protected bool is_my_turn = false;

    [Header("Sheet")]
    [SerializeField] protected List<SkillData> skills_unlocked = new List<SkillData>();
    [SerializeField] protected List<SkillData> skill_equipped = new List<SkillData>();
    [SerializeField] protected int strength, dexterity, constitution, intelligence, mind;
    [SerializeField] protected int life_max, life_current, mana_max, mana_current;
    [SerializeField] protected int r_slashing, r_bludgeoning, r_pierce, r_fire, r_ice, r_lightning, r_purity;

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
    public void SetBattleChar(BattleCharSO battle_char)
    {
        this.battle_char = battle_char; 
    }
    #region Init
    public virtual void InitStats()
    {
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
                Debug.Log($"Tomou {dmg} de dano");
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
        if (skill_current == null) yield break;
        StartCoroutine(GenericTools.ForceAnimation(animator, skill_current.skill.clip));
        yield return StartCoroutine(OnGoingSkill());
    }

    public virtual IEnumerator OnGoingSkill()
    {
        yield return null;
    }

    public virtual IEnumerator FinishingSkill()
    {
        ExecuteSkillsEffects();
        skill_current.cooldown = skill_current.skill.cooldown;
        yield return null;
    }

    public virtual void ExecuteSkillsEffects()
    {
        if (skill_current == null) return;
        foreach(SkillEffectSO f in skill_current.skill.effects)
        {
            f.Execute(ctx);
        }
    }

    protected void ReduceCooldown()
    {
        foreach(SkillData dt in skill_equipped)
        {
            dt.cooldown = Mathf.Clamp(dt.cooldown - 1, 0, 99);
        }
    }

    public void AddSkill(SkillSO skill)
    {
        SkillData data = new SkillData(skill);

        skills_unlocked.Add(data);
    }

    public void EquipSkill(SkillData data)
    {
        if(skill_equipped.Contains(data)) return;
        data.cooldown = 0;
        skill_equipped.Add(data);
    }
    public void UnequipSkill(SkillData data)
    {
        if (!skill_equipped.Contains(data)) return;
        skill_equipped.Remove(data);
    }
    #endregion

    public void CreateContext(List<BattleChar> allies, List<BattleChar> enemies)
    {
        ctx = new BattleContext();
        ctx.Target = this;  //debug
        ctx.Self = this;
        ctx.Party = allies;
        ctx.Enemies = enemies;
    }
}
