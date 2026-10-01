using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Playables;


public enum StatType
{
    SPD,
    DMG_MIN,
    DMG_MAX,
    CRIT,
    PROTECT,
    DODGE,
    ACCURACY,
    LIFE_MAX,
    LIFE_CURRENT,
    R_STUN,
    R_MOVE,
    R_DEBUFF,
    R_BLEED,
    R_POISON,
    R_SANITY,
    NOONE,
}

public abstract class BattleChar : MonoBehaviour, IPointerClickHandler
{
    public Transform tf { get ; private set; }
    public SpriteRenderer sprite_renderer { get; private set; }

    [Header("BattleInfo")]
    [SerializeField] protected int life_max;
    [SerializeField] protected int life_current;
    [SerializeField] protected int protect, dodge, damage_min, damage_max, accuracy, spd, crit;
    public List<StatusResistence> Resistences = new List<StatusResistence>();
    [SerializeField] protected BattleCharSO battle_char;
    [SerializeField] protected BattleContext ctx;
    [SerializeField] protected List<FavoritePos> favorite_pos = new();

    public CharacterSpriteSO spriteSO { get; private set; }

    [SerializeField] protected List<StatusEffectData> StatusEffect = new();
    [SerializeField] protected List<StatsModifierSO> StatsModifier = new();
    public IReadOnlyList<StatusEffectData> status_effects => StatusEffect.AsReadOnly();
    List<StatusEffectData> onlyapply = new();

    
    public BattleContext Context => ctx;
    [SerializeField] protected SkillSO skill_current;
    [SerializeField] protected TurnStep step;
    protected BattleManager battle_manager;
    
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

        Resistences.Add(new StatusResistence(ResistenceType.STUN, battle_char.stun));
        Resistences.Add(new StatusResistence(ResistenceType.DEBUFF, battle_char.debuff));
        Resistences.Add(new StatusResistence(ResistenceType.BLEED, battle_char.bleed));
        Resistences.Add(new StatusResistence(ResistenceType.POISON, battle_char.poison));
        Resistences.Add(new StatusResistence(ResistenceType.MOVE, battle_char.move));
        Resistences.Add(new StatusResistence(ResistenceType.SANITY, battle_char.sanity));
    }

    public virtual void SetFavoritePos(List<SkillSO> so_list)
    {
        favorite_pos.Clear();
        for(int ii = 0;  ii < 4; ii++)
        {
            favorite_pos.Add(new FavoritePos());
        }

        foreach (SkillSO so in so_list)
        {
            for(int i = 0; i < so.use_pos.Length; i++)
            {
                favorite_pos[so.use_pos[i]].AddSkill(so);
            }
        }
    }

    public void SetBattleChar(BattleCharSO battle_char)
    {
        this.battle_char = battle_char;
    }

    public void SetBattleManager(BattleManager battle)
    {
        battle_manager = battle;
    }
    #endregion

    #region Generic
    public void TakeDamage(DamageInfo info)
    {
        Debug.Log($"dano pré defesa: {info.damage}");
        if(!info.ignore_prot) ReduceDamage(info);
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
        data.source = this;
        data.owner.ReceiveStatus(data);
    }

    public void ReceiveStatus(StatusEffectData data)
    {
        if (data.sfSO.resistable == true) {
            foreach (StatusResistence st in Resistences)
            {
                if (st.resist == data.sfSO.type)
                {
                    int roll = Random.Range(0, 101);
                    if (roll < st.value) {return; }
                }
            } 
        }
        OnReceiveStatus(data);
        if(data.stacks == 0) { data.OnApply(); return; }   
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

    public void ReceiveStatsModifier(StatsModifierSO modifier)
    {
        if(modifier != null)
        {
            StatsModifier.Add(modifier);
        }
    }

    public void RemoveStatsModifier(StatsModifierSO modifier)
    {
        if (StatsModifier.Contains(modifier))
        {
            StatsModifier.Remove(modifier);
        }
    }

    public int GetStatsModifier(StatType t)
    {
        List<StatsModifierSO> flat = StatsModifier.FindAll(x => x.flat == true && x.statType == t);
        List<StatsModifierSO> percent = StatsModifier.FindAll(x => x.flat == false && x.statType == t);

        int value = 0;
        foreach (StatsModifierSO modifier in flat)
        {
            value += modifier.value;
        }
        foreach(StatsModifierSO modifier in percent)
        {
            value += (int)(GetBaseStat(modifier.statType) * (modifier.value / 100f));
        }
        return value;   
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

    public void OnUseSkill(BattleResult result)
    {
        foreach (StatusEffectData status in onlyapply)
        {
            status.OnUseSkill(result);
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
        context.skillso = skill_current;
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
        int roll = Random.Range(0, 101);
        result.acc_base = skill_current.acc_base + accuracy;
        result.acc_roll = roll;
        OnUseSkill(result);
        result.skill_fail = roll <= skill_current.acc_base || !skill_current.can_miss ? false : true;
        result.crit = roll <= crit + skill_current.crit_mod;
        Debug.Log($"{result.acc_roll} {result.acc_base}");

        foreach (BattleChar n in ctx.Target)
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

    protected abstract void ChoosingSkill();

    protected abstract void ChoosingTarget();

    public abstract void SelectTarget(int index);
    public abstract void ConfirmTarget();
    public abstract void ChooseSwapTarget();
    public void ConfirmSwapTarget(int index)
    {
        if (battle_manager == null) return;
        if (index == ctx.SelfPosition || index < 0 || index >= ctx.Party.Count) return;

        BattleChar target = ctx.Party[index];

        if (target == null) return;

        battle_manager.SwapPosition(this, target);

        step = TurnStep.Finish;
    }

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

    public void SetStep(TurnStep s)
    {
        step = s;
    }

    public virtual void SetRound()
    {
        if(ctx.Target.Count >= 1) ctx.Target.Clear();
        OnStartTurn();
        battle_manager.RefreshHUD();
    }

    public TurnStep CurrentStep()
    {
        return step;
    }

    public int GetSelfPos()
    {
        return ctx.SelfPosition;
    }



    public int GetBaseStat(StatType stat)
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
            StatType.CRIT => crit,
            StatType.R_STUN => Resistences[0].value,
            StatType.R_DEBUFF => Resistences[1].value,
            StatType.R_BLEED => Resistences[2].value,
            StatType.R_POISON => Resistences[3].value,
            StatType.R_MOVE => Resistences[4].value,
            StatType.R_SANITY => Resistences[5].value,
            StatType.NOONE => 0,
            _ => 0
        };
    }

    public int GetStat(StatType stat)
    {
        int baseValue = GetBaseStat(stat);
        int modifier = GetStatsModifier(stat);

        return baseValue + modifier;
    }

    #endregion 

    public void OnPointerClick(PointerEventData eventData)
    {
        battle_manager.CurrentSetTarget(this);
    }

    protected virtual void Awake()
    {
        sprite_renderer = GetComponent<SpriteRenderer>();
        tf = GetComponent<Transform>(); 
    }
}
