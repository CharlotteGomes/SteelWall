using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;



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
    MOVE,
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
    public bool its_alive_form = true;
    [SerializeField] protected int life_max;
    [SerializeField] protected int life_current;
    [SerializeField] protected int protect, dodge, damage_min, damage_max, accuracy, spd, crit, move;
    public List<StatusResistence> Resistences = new List<StatusResistence>();
    [SerializeField] protected BattleCharData battle_char;
    [SerializeField] protected BattleContext ctx;
    [SerializeField] protected List<FavoritePos> favorite_pos = new();

    public CharacterSpriteSO spriteSO { get; private set; }

    [SerializeField] protected List<StatusEffectData> StatusEffect = new();
    [SerializeField] protected List<StatsModifierSO> StatsModifier = new();
    
    public IReadOnlyList<StatusEffectData> status_effects => StatusEffect.AsReadOnly();
    
    public BattleContext Context => ctx;
    [SerializeField] protected SkillSO skill_current;
    [SerializeField] protected SkillData skill_current_data;
    [SerializeField] protected TurnStep step;
    protected BattleManager battle_manager;

    [Header("Actions")]
    public Action<BattleChar> OnStartMyTurn;

    public Action<SkillSO> OnSkillSelect;
    public Action<SkillData> OnSkillConfirm;
    public Action OnSkillCancel;

    public Action<BattleChar[]> OnBattleCharSelect;

    public Action OnMoveActionSelect;
    public Action<BattleContext> OnMoveConfirm;
    public Action OnMoveCancel;
    public Action<BattleChar> OnSwapTargetSelect;

    public Action OnSkipSelect;

    public Action OnEndMyTurn;

    public Action<BattleChar> OnDeath;

    #region Init
    public virtual void InitStats()
    {
        name = battle_char.Name;
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
        move = battle_char.move_range;
        sprite_renderer.sprite = spriteSO.idle;

        Resistences.Clear();
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
        if (so_list == null) return;
        for(int ii = 0;  ii < 4; ii++)
        {
            favorite_pos.Add(new FavoritePos());
        }

        foreach (SkillSO so in so_list)
        {
            if (so == null || so.use_pos == null) continue;
            for(int i = 0; i < so.use_pos.Length; i++)
            {
                int position = so.use_pos[i];
                if (position < 0 || position >= favorite_pos.Count) continue;
                favorite_pos[position].AddSkill(so);
            }
        }
    }

    public void SetBattleChar(BattleCharData battle_char)
    {
        this.battle_char = battle_char;
    }

    public void SetBattleManager(BattleManager battle)
    {
        battle_manager = battle;
    }
    public virtual void CreateContext(List<BattleChar> allies, List<BattleChar> enemies, int pos)
    {
        if (ctx == null) ctx = new BattleContext();
        ctx.Self = this;
        ctx.Party = allies;
        ctx.Enemies = enemies;
        ctx.SelfPosition = pos;
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
        Debug.Log($"ME CURARAM EM {info.heal_amount}");
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
        data.owner.ReceiveStatus(data);
    }

    public void ReceiveStatus(StatusEffectData data)
    {
        Debug.Log(data.effectName);
        OnReceiveStatus(data);
        if (data.resistiable == true) {
            foreach (StatusResistence st in Resistences)
            {
                if (st.resist == data.sfSO.type)
                {
                    int roll = UnityEngine.Random.Range(0, 101);
                    if (roll < st.value) {return; }
                }
            } 
        }
        if(data.stacks == 0) { data.OnApply(); return; }
        foreach (var s in StatusEffect)
        {
            if (data.effectName == s.effectName) { s.OnStackPile(data); return; }
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

    public void CharacterDeath()
    {
        OnCharacterDeath();
        OnDeath?.Invoke(this);
        Destroy(gameObject);
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
        List<StatsModifierSO> flat = StatsModifier.FindAll(x => x.flat == true && x.statType == t).ToList();
        List<StatsModifierSO> percent = StatsModifier.FindAll(x => x.flat == false && x.statType == t).ToList();

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
        foreach(StatusEffectData status in StatusEffect.ToList())
        {
            status.OnStartTurn();
        }
        CheckStatusEffect();
    }

    public void OnEndTurn()
    {
        foreach (StatusEffectData status in StatusEffect.ToList())
        {
            status.OnEndTurn();
        }
        CheckStatusEffect();
    }

    public void OnUseSkill(BattleResult result)
    {
        foreach (StatusEffectData status in StatusEffect.ToList())
        {
            status.OnUseSkill(result);
        }
        CheckStatusEffect();
    }
    public void OnDamageDealt(DamageInfo info)
    {
        foreach (StatusEffectData status in StatusEffect.ToList())
        {
            status.OnDamageDealt(info);
        }
        CheckStatusEffect();
    }

    public void OnDamageTaken(DamageInfo info)
    {
        foreach (StatusEffectData status in StatusEffect.ToList())
        {
            status.OnDamageTaken(info);
        }
        CheckStatusEffect();
    }

    public void OnHealGiven(HealInfo info)
    {
        foreach(StatusEffectData status in StatusEffect.ToList())
        {
            status.OnHealGiven(info);
        }
        CheckStatusEffect();
    }
    public void OnHealReceived(HealInfo info) 
    {
        foreach(StatusEffectData status in StatusEffect.ToList())
        {
            status.OnHealReceived(info);
        }
        CheckStatusEffect();
    }
    public void OnReceiveStatus(StatusEffectData status)
    {
        foreach(StatusEffectData s in StatusEffect.ToList())
        {
            s.OnReceiveStatus(status);
        }
        CheckStatusEffect();
    }

    public void OnGiveStatus(StatusEffectData status)
    {
        foreach(StatusEffectData s in StatusEffect.ToList())
        {
            s.OnGiveStatus(status);
        }
        CheckStatusEffect();
    }   

    public void OnStatusRemoved(StatusEffectData status)
    {
        foreach(StatusEffectData s in StatusEffect.ToList())
        {
            s.OnStatusRemoved(status);
        }
        CheckStatusEffect();
    }

    public void OnCharacterDeath() {
        foreach(StatusEffectData status in StatusEffect.ToList())
        {
            status.OnCharacterDeath();
        }
        CheckStatusEffect();
    }

    public void CheckStatusEffect()
    {
        List<StatusEffectData> s = StatusEffect.ToList();
        foreach (StatusEffectData status in s)
        {
            if(status.stacks <= 0) {StatusRemove(status); }
        }
    }
    #endregion

    #region Skill functions
    public virtual IEnumerator StartSkill()
    {
        BattleResult result = new BattleResult();
        yield return StartCoroutine(OnGoingSkill(result));
    }

    public virtual IEnumerator OnGoingSkill(BattleResult result) 
    {
        ResolveSkill(result);
        yield return StartCoroutine(FinishingSkill(result));
    }

    public virtual IEnumerator FinishingSkill(BattleResult result)
    {
        if (!result.skill_fail && skill_current_data != null)
            skill_current_data.Execute(this, result);

        skill_current_data?.Dispose();
        skill_current_data = null;
        yield return null;
    }

    public void ResolveSkill(BattleResult result)
    {
        int roll = UnityEngine.Random.Range(0, 101);
        result.acc_base = skill_current_data.acc_base + GetStat(StatType.ACCURACY);
        result.acc_roll = roll;
        OnUseSkill(result);
        result.skill_fail = result.acc_roll <= result.acc_base || !skill_current.can_miss ? false : true;
        result.crit = roll <= GetStat(StatType.CRIT) + skill_current_data.crit_mod && skill_current_data.original_skill.can_crit;
        Debug.Log($"{result.acc_roll} {result.acc_base}");

        result.hit.Clear();
        result.dodge.Clear();
        foreach (BattleChar n in ctx.Target)
        {
            if (n == null || !n.CheckAlive()) continue;
            int hitChance = result.acc_base - n.GetStat(StatType.DODGE);
            if (result.acc_roll > hitChance && skill_current_data.original_skill.can_dodge)
            {
                Debug.Log($"{n.name} desviou");
                result.dodge.Add(n);
                continue;
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
    public virtual IEnumerator OnGoingTurn() { yield return null; }

    public virtual void SelectTarget(int index)
    {
        if (skill_current_data == null) { return; }
        if (index < 0 || index >= skill_current_data.targets.Count) return;
        ctx.Target.Clear();
        if (!skill_current.chose)
        {
            ctx.Target = skill_current_data.targets.ToList();
            skill_current_data.Prepare();
            return;
        }

        ctx.Target.Add(skill_current_data.targets[index]);
        skill_current_data.Prepare();
    }
    public virtual void ChooseSwapTarget() { }
    public void ConfirmSwapTarget(int index)
    {
        if (battle_manager == null) return;
        if (index == ctx.SelfPosition || index < 0 || index >= ctx.Party.Count) return;
        battle_manager.SwapPosition(this, index);
        step = TurnStep.Finish;
    }

    #endregion

    #region Turn Info

    public void SetStep(TurnStep s)
    {
        step = s;
    }

    public void SetTarget(BattleChar c)
    {
        if (c == null || skill_current_data == null || ctx == null) return;
        if (!skill_current_data.targets.Contains(c)) return;
        ctx.Target.Clear();
        ctx.Target.Add(c);
    }
    
    public bool CanUseSkill(SkillSO so)
    {
        if (so == null || so.use_pos == null || ctx == null) return false;
        for (int i = 0; i < so.use_pos.Length; i++)
        {
            if (ctx.SelfPosition == so.use_pos[i]) return true;
        }
        return false;
    }

    #endregion

    #region Status
    

    public int GetSelfPos()
    {
        return ctx.SelfPosition;
    }

    private int GetResistance(ResistenceType type)
    {
        StatusResistence resistance = Resistences.Find(item => item.resist == type);
        return resistance != null ? resistance.value : 0;
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
            StatType.MOVE => move,
            StatType.R_STUN => GetResistance(ResistenceType.STUN),
            StatType.R_DEBUFF => GetResistance(ResistenceType.DEBUFF),
            StatType.R_BLEED => GetResistance(ResistenceType.BLEED),
            StatType.R_POISON => GetResistance(ResistenceType.POISON),
            StatType.R_MOVE => GetResistance(ResistenceType.MOVE),
            StatType.R_SANITY => GetResistance(ResistenceType.SANITY),
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

    #region Round
    public virtual void SetRound()
    {
        if (ctx.Target.Count >= 1) ctx.Target.Clear();
        OnStartTurn();
        OnStartMyTurn?.Invoke(this);
    }

    public virtual void EndRound()
    {
        OnEndMyTurn?.Invoke();
    }

    public TurnStep CurrentStep()
    {
        return step;
    }
    #endregion

    public void OnPointerClick(PointerEventData eventData)
    {
    }

    protected virtual void Awake()
    {
        sprite_renderer = GetComponent<SpriteRenderer>();
        tf = GetComponent<Transform>(); 
    }
}
