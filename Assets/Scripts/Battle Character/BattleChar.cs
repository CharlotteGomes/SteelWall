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
    public Transform tf { get ; private set; }
    public SpriteRenderer sprite_renderer { get; private set; }

    [Header("BattleInfo")]
    [SerializeField] protected int life_max;
    [SerializeField] protected int life_current;
    [SerializeField] protected BattleCharSO battle_char;
    [SerializeField] protected BattleContext ctx;
    public BattleContext Context => ctx;
    [SerializeField] protected SkillSO skill_current;
    [SerializeField] protected List<StatusEffectData> StatusEffect = new();
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

    public IReadOnlyList<StatusEffectData> status_effects => StatusEffect.AsReadOnly();
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
            if (dt.resistence_type == info.damageType)
            {
                info.damage = Mathf.Clamp(info.damage * (1 - dt.value), 0, Mathf.Infinity);
                Debug.Log(info.damage);
            }
        }
    }

    public void TakeDamage(DamageInfo info)
    {
        Debug.Log($"{info.receiver.battle_char.name} took {info.damage} damage from {info.dealer.battle_char.name}");
        ApplyResistences(info);
        OnDamageTaken(info);
        info.damageType.OnDamageTaken(info);
        if(info.damage>= 0)
        {
            life_current = (int)Mathf.Clamp(life_current - info.damage, 0, Mathf.Infinity);
        }
    }

    public void DealDamage(DamageInfo info)
    {
        Debug.Log($"{info.dealer.battle_char.name} dealt {info.damage} damage to {info.receiver.battle_char.name}");
        OnDamageDealt(info);
        info.damageType.OnDamageDealt(info);    
        if(info.damage >= 0)
        {
            info.receiver.TakeDamage(info);
        }
    }

    public void ReceiveHeal(HealInfo info)
    {
        Debug.Log($"{info.target.battle_char.name} received {info.heal_amount} healing from {info.healer.battle_char.name}");
        OnHealReceived(info);
        if(info.heal_amount >= 0)
        {
            life_current = (int)Mathf.Clamp(life_current + info.heal_amount, 0, life_max);
        }
    }

    public void DealHeal(HealInfo info)
    {
        Debug.Log($"{info.healer.battle_char.name} healed {info.target.battle_char.name} for {info.heal_amount}");
        OnHealGiven(info);
        if(info.heal_amount >= 0)
        {
            info.target.ReceiveHeal(info);
        }
    }

    public void GiveStatus(StatusEffectData data)
    {
        OnGiveStatus(data);
        data.OnApply();
        data.source = this;
        data.owner.ReceiveStatus(data);
    }

    public void ReceiveStatus(StatusEffectData data)
    {
        OnReceiveStatus(data);
        foreach (StatusEffectData dt in StatusEffect)
        {
            if (data.effectName == dt.effectName)
            {
                int amount = data.stacks > dt.stacks ? data.stacks : dt.stacks;
                dt.stacks = amount;
                continue;
            }
        }
        StatusEffect.Add(data);
        data.OnApply();
    }

    public void StatusRemove(StatusEffectData status)
    {
        if (StatusEffect.Contains(status))
        {
            StatusEffect.Remove(status);
            status.OnRemove();
        }
        OnStatusRemoved(status);
    }

    public bool CheckAlive()
    {
        return life_current <= 0 ? false : true;
    }
    #endregion

    #region Turn Events
    public void OnStartTurn()
    {
        foreach(StatusEffectData status in StatusEffect)
        {
            status.OnStartTurn();
        }
    }

    public void OnEndTurn()
    {
        foreach (StatusEffectData status in StatusEffect)
        {
            status.OnEndTurn();
        }
    }

    public void OnDamageDealt(DamageInfo info)
    {
        foreach (StatusEffectData status in StatusEffect)
        {
            status.OnDamageDealt(info);
        }
    }

    public void OnDamageTaken(DamageInfo info)
    {
        foreach (StatusEffectData status in StatusEffect)
        {
            status.OnDamageTaken(info);
        }
    }

    public void OnHealGiven(HealInfo info)
    {
        foreach(StatusEffectData status in StatusEffect)
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
        foreach(StatusEffectData status in StatusEffect)
        {
            status.OnHealReceived(info);
        }
    }
    public void OnReceiveStatus(StatusEffectData status)
    {
        foreach(StatusEffectData s in StatusEffect)
        {
            s.OnReceiveStatus(status.sfSO);
        }
    }

    public void OnGiveStatus(StatusEffectData status)
    {
        foreach(StatusEffectData s in StatusEffect)
        {
            s.OnGiveStatus(status.sfSO);
        }
    }   

    public void OnStatusRemoved(StatusEffectData status)
    {
        foreach(StatusEffectData s in StatusEffect)
        {
            s.OnStatusRemoved(status.sfSO);
        }
    }

    public void OnCharacterDeath() {
        foreach(StatusEffectData status in StatusEffect)
        {
            status.OnCharacterDeath();
        }
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
        if(ctx == null) ctx = new BattleContext();
        ctx.Self = this;
        ctx.Party = allies;
        ctx.Enemies = enemies;
    }

    public void SetTarget(int pos, List<BattleChar> list)
    {
        if(pos > list.Count - 1)
        {
            Debug.LogError("Target position is out of range");
            return;
        }
        ctx.Target.Add(list[pos]);
    }

    public virtual void SetRound()
    {
        if(ctx.Target.Count >= 1) ctx.Target.Clear();
        OnStartTurn();
    }

    protected virtual void Awake()
    {
        sprite_renderer = GetComponent<SpriteRenderer>();
        tf = GetComponent<Transform>(); 
    }
}
