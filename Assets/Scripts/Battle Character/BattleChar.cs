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
using UnityEngine.Rendering;

/*public enum BattleOptions 
{ 
    Attack = 0,
    Skill = 1,
}*/
public abstract class BattleChar : MonoBehaviour
{
    
    [Header("BattleInfo")]
    [SerializeField] protected int life_max;
    [SerializeField] protected int life_current;
    [SerializeField] protected BattleCharSO battle_char;
    [SerializeField] protected BattleContext ctx;
    [SerializeField] protected SkillSO skill_current;
    [SerializeField] protected List<StatusEffectSO> StatusEffect = new();
    [SerializeField] protected TurnStep step;
    public CharacterSpriteSO spriteSO { get; private set; }
    

    public int strength { get; protected set; }
    public int dexterity { get; protected set; }
    public int constitution { get; protected set; }
    public int intelligence { get; protected set; }
    public int mind { get; protected set; }
    public int r_slashing { get; protected set; }
    public int r_bludgeoning { get; protected set; }
    public int r_pierce { get; protected set; }
    public int r_fire { get; protected set; }
    public int r_ice { get; protected set; }
    public int r_lightning { get; protected set; }
    public int r_purity { get; protected set; }

[Header("Ref")]
    [SerializeField] Animator animator;

    public IReadOnlyList<StatusEffectSO> status_effects => StatusEffect.AsReadOnly();
    /*[Header("Actions")]
    public Action<BattleChar> Death;
    public Action<BattleChar> start_turn;
    public Action<BattleChar> end_turn;
    public Action<BattleChar, TypeDmg, int> deal_damage;
    public Action<BattleChar, TypeDmg, int> take_damage;
    public Action<BattleChar, int> deal_heal;
    public Action<BattleChar, int> receive_heal;
    public Action<BattleChar> apply_status_effect;
    public Action<BattleChar> receive_status_effect;*/

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
    public void SetBattleChar(BattleCharSO battle_char)
    {
        this.battle_char = battle_char;
    }
    #endregion

    #region Generic

    public void ApplyResistences(DamageInfo info)
    {
        foreach(ResistenseData dt in battle_char.Resists)
        {
            if(dt.resistence_type == info.damageType)
            {
                info.damage = info.damage * (1 - dt.value);
                Debug.Log(info.damage);
            }
        }
    }

    public void TakeDamage(DamageInfo info)
    {
        ApplyResistences(info);
        OnDamageTaken(info);
        info.damageType.OnDamageTaken(info);
        if(info.damage> 0)
        {
            life_current -= (int)info.damage;
        }
    }

    public void DealDamage(DamageInfo info)
    {
        OnDamageDealt(info);
        info.damageType.OnDamageDealt(info);    
        if(info.damage > 0)
        {
            info.receiver.TakeDamage(info);
        }
    }

    public void OnStartTurn()
    {
        foreach(StatusEffectSO status in StatusEffect)
        {
            status.OnStartTurn();
        }
    }

    public void OnEndTurn()
    {
        foreach (StatusEffectSO status in StatusEffect)
        {
            status.OnEndTurn();
        }
    }

    public void OnDamageDealt(DamageInfo info)
    {
        //Debug.Log($"{this.name} deu {info.damage} de dano base em {info.receiver.name} | Procurando por passivas para aumentar o dano");
        foreach (StatusEffectSO status in StatusEffect)
        {
            status.OnDamageDealt(info);
        }
    }

    public void OnDamageTaken(DamageInfo info)
    {
        //Debug.Log($"{this.name} recebeu {info.damage} de dano pós melhorias de {info.dealer.name} | Procurando por passivas para reduzir o dano");
        foreach (StatusEffectSO status in StatusEffect)
        {
            status.OnDamageTaken(info);
        }
        //Debug.Log($"{this.name} recebeu {info.damage} de dano final de {info.dealer.name}");
    }

    public void OnHealGiven(HealInfo info)
    {
        foreach(StatusEffectSO status in StatusEffect)
        {
            status.OnHealGiven(info);
        }
        if(info.heal_amount > 0)
        {
            foreach(BattleChar target in ctx.Target)
            {
                target.OnHealReceived(info);
            }
        }
    }
    public void OnHealReceived(HealInfo info) 
    {
        foreach(StatusEffectSO status in StatusEffect)
        {
            status.OnHealReceived(info);
        }
    }
    public void OnStatusApplied(StatusEffectSO status)
    {
        foreach(StatusEffectSO s in StatusEffect)
        {
            s.OnStatusApplied(status);
        }
    }
    public void OnStatusRemoved(StatusEffectSO status)
    {
        foreach(StatusEffectSO s in StatusEffect)
        {
            s.OnStatusRemoved(status);
        }
    }

    public void StatusApply(StatusEffectSO status)
    {
        if (!StatusEffect.Contains(status)) 
        { 
        StatusEffect.Add(status);
        status.OnApply();
        }

        OnStatusApplied(status);
    }

    public void StatusRemove(StatusEffectSO status)
    {
        if (StatusEffect.Contains(status))
        {
            StatusEffect.Remove(status);
            status.OnRemove();
        }
        OnStatusRemoved(status);
    }
    #endregion

    #region Skill functions
    public virtual IEnumerator StartSkill()
    {
        yield return StartCoroutine(OnGoingSkill());
    }

    public virtual IEnumerator OnGoingSkill()
    {
        yield return StartCoroutine(GM.instance.battle_manager.battle_presentation.StartPresentation(ctx.Self, ctx.Target, skill_current.presentation));
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

    public void SetTarget(BattleChar target)
    {
        ctx.Target.Add(target);
    }

    public virtual void SetRound()
    {
        if(ctx.Target.Count >= 1) ctx.Target.Clear();
    }
}
