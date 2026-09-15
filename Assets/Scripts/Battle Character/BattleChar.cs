using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;


public enum StatType
{
    SPD,
    DMG_MIN,
    DMG_MAX,
    PROTECT,
    DODGE,
    ACCURACY,
    LIFE_MAX,
    LIFE_CURRENT
}

public abstract class BattleChar : MonoBehaviour
{
    public Transform tf { get ; private set; }
    public SpriteRenderer sprite_renderer { get; private set; }

    [Header("BattleInfo")]
    [SerializeField] protected int life_max;
    [SerializeField] protected int life_current;
    [SerializeField] protected int protect, dodge, damage_min, damage_max, accuracy, spd, crit;
    [SerializeField] protected BattleCharSO battle_char;
    [SerializeField] protected BattleContext ctx;
    public CharacterSpriteSO spriteSO { get; private set; }

    [SerializeField] protected List<StatusEffectData> StatusEffect = new();
    public IReadOnlyList<StatusEffectData> status_effects => StatusEffect.AsReadOnly();
    List<StatusEffectData> onlyapply = new();

    public BattleContext Context => ctx;
    [SerializeField] protected SkillSO skill_current;
    [SerializeField] protected TurnStep step;

    #region Init
    public virtual void InitStats()
    {
        spriteSO = battle_char.sprites;
        life_max = battle_char.life;
        life_current = life_max;
        protect = battle_char.protect;
        dodge = battle_char.dodge;
        damage_min = battle_char.damage_min;
        damage_max = battle_char.damage_max;
        accuracy = battle_char.accuracy;
        spd = battle_char.spd;
        crit = battle_char.crit;
        sprite_renderer.sprite = spriteSO.idle;
    }
    public void SetBattleChar(BattleCharSO battle_char)
    {
        this.battle_char = battle_char;
    }
    #endregion
    //divide por 100 e multiplica pelo valor total

    #region Generic
    public void TakeDamage(DamageInfo info)
    {
        Debug.Log($"dano pré defesa: {info.damage}");
        if(info.ignore_prot) ReduceDamage(info);
        Debug.Log($"dano pós defese: {info.damage}");
        OnDamageTaken(info);
        if(info.damage>= 0)
        {
            life_current = (int)Mathf.Clamp(life_current - info.damage, 0, Mathf.Infinity);
        }
    }

    public void DealDamage(DamageInfo info)
    {
        OnDamageDealt(info);   
        if(info.damage >= 0)
        {
            info.receiver.TakeDamage(info);
        }
    }

    public void ReduceDamage(DamageInfo info)
    {
        if (info.damage >= 0)
        {
            float prot = protect;
            Debug.Log(info.damage * prot / 100);
            info.damage -= (int)(info.damage * (prot / 100));
        }
    }

    public void ReceiveHeal(HealInfo info)
    {
        OnHealReceived(info);
        if(info.heal_amount >= 0)
        {
            life_current = (int)Mathf.Clamp(life_current + info.heal_amount, 0, life_max);
        }
    }

    public void DealHeal(HealInfo info)
    {
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
                dt.stacks += data.stacks;
                return;
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
        onlyapply = StatusEffect.ToList();
        foreach(StatusEffectData status in onlyapply)
        {
            status.OnStartTurn();
        }
    }

    public void OnEndTurn()
    {
        foreach (StatusEffectData status in onlyapply)
        {
            status.OnEndTurn();
        }
    }

    public void OnDamageDealt(DamageInfo info)
    {
        foreach (StatusEffectData status in onlyapply)
        {
            status.OnDamageDealt(info);
        }
    }

    public void OnDamageTaken(DamageInfo info)
    {
        foreach (StatusEffectData status in onlyapply)
        {
            status.OnDamageTaken(info);
        }
    }

    public void OnHealGiven(HealInfo info)
    {
        foreach(StatusEffectData status in onlyapply)
        {
            status.OnHealGiven(info);
        }
    }
    public void OnHealReceived(HealInfo info) 
    {
        foreach(StatusEffectData status in onlyapply)
        {
            status.OnHealReceived(info);
        }
    }
    public void OnReceiveStatus(StatusEffectData status)
    {
        foreach(StatusEffectData s in onlyapply)
        {
            s.OnReceiveStatus(status.sfSO);
        }
    }

    public void OnGiveStatus(StatusEffectData status)
    {
        foreach(StatusEffectData s in onlyapply)
        {
            s.OnGiveStatus(status.sfSO);
        }
    }   

    public void OnStatusRemoved(StatusEffectData status)
    {
        foreach(StatusEffectData s in onlyapply)
        {
            s.OnStatusRemoved(status.sfSO);
        }
    }

    public void OnCharacterDeath() {
        foreach(StatusEffectData status in onlyapply)
        {
            status.OnCharacterDeath();
        }
    }
    #endregion


    #region Skill functions
    public virtual IEnumerator StartSkill()
    {
        BattlePresentation.CreateGraph();
        PresentationContext context = new PresentationContext(ctx.Self, ctx.Target.ToArray(), Camera.main, skill_current.presentation);
        BattleResult result = new BattleResult();

        yield return StartCoroutine(OnGoingSkill(result));
        result.AddBuild(new BuildAttackStage(context));
        result.AddBuild(new BuildReturnStage(context));
        var presentation = result.BuildFinalResult();

        var output = ScriptPlayableOutput.Create(BattlePresentation.graph, "Battle Presentation");

        output.SetSourcePlayable(presentation);
        BattlePresentation.graph.Play();
        yield return new WaitForSeconds((float)presentation.GetDuration());
        BattlePresentation.graph.Destroy();
    }

    public virtual IEnumerator OnGoingSkill(BattleResult result) 
    {
        int roll = Random.Range(0, 101);

        result.acc_base = skill_current.acc_base + accuracy;
        result.acc_roll = roll;
        result.skill_fail = roll <= skill_current.acc_base || !skill_current.can_miss ? false : true;
        result.crit = roll <= crit + skill_current.crit_mod;
        Debug.Log($"{result.acc_roll} {result.acc_base} {result.skill_fail}");
        ResolveSkill(result);
        yield return StartCoroutine(FinishingSkill(result));
    }

    public virtual IEnumerator FinishingSkill(BattleResult result)
    {
        if (result.skill_fail) yield break ;
        skill_current.Execute(this, result);
        yield return null;
    }

    public void ResolveSkill(BattleResult result)
    {
        foreach(BattleChar n in ctx.Target)
        {
            int hitChance = result.acc_base - n.dodge;

            if (result.acc_roll > hitChance)
            {
                Debug.Log($"{n.name} desviou");
                result.dodge.Add(n);
                return;            
            }
            Debug.Log($"{n.name} tomou a skill");
            result.hit.Add(n);
        }
    }

    public void SetSkill(SkillSO so)
    {
        skill_current = so;
    }
    #endregion

    #region Turn
    public abstract IEnumerator OnGoingTurn();
    #endregion

    #region Info
    public void CreateContext(List<BattleChar> allies, List<BattleChar> enemies, int pos)
    {
        if(ctx == null) ctx = new BattleContext();
        ctx.Self = this;
        ctx.Party = allies;
        ctx.Enemies = enemies;
        ctx.SelfPosition = pos;
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

    public int GetStat(StatType stat)
    {
        return stat switch
        {
            StatType.SPD => spd,
            StatType.DMG_MIN => damage_min,
            StatType.DMG_MAX => damage_max,
            StatType.PROTECT => protect,
            StatType.LIFE_CURRENT => life_current,
            StatType.LIFE_MAX => life_max,
            StatType.ACCURACY => accuracy,
            StatType.DODGE => dodge,
            _ => 0
        };
    }

    #endregion 
    protected virtual void Awake()
    {
        sprite_renderer = GetComponent<SpriteRenderer>();
        tf = GetComponent<Transform>(); 
    }
}
