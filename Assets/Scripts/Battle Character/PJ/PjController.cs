using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;
public class PjController : BattleChar
{
    [SerializeField] List<SkillSO> test = new List<SkillSO>();

    [SerializeField] protected List<SkillSO> skills_unlocked = new List<SkillSO>();
    [SerializeField] protected List<SkillSO> skills_equipped = new List<SkillSO>();
    public IReadOnlyList<SkillSO> read_skills => skills_equipped;

    public int skill_index { get; private set; } = 0;
    public int intent_index { get; private set; } = 0;
    protected override void Awake()
    {
        base.Awake();
        life_max = 1000;
        life_current = 1000;
        /*StatusEffectSO pois = new StatusEffectSO();
        pois.stacks = 3;
        StatusEffectData instance = new PoisonEffect(this, this, pois, 3);
        GiveStatus(instance);*/
    }

    void Start()
    {
        foreach (SkillSO wo in test)
        {
            AddSkill(wo);
        }
        skills_equipped = skills_unlocked;
    }

    // Update is called once per frame
    void Update()
    {
    }

    #region Init
    public override void InitStats()
    {
        base.InitStats();
    }
    #endregion

    #region Skill Functions
    protected void ReduceCooldown()
    {
        foreach (SkillSO dt in skills_equipped)
        {
        }
    }

    public void AddSkill(SkillSO data)
    {
        if(skills_unlocked.Contains(data)) return;
        skills_unlocked.Add(data);
    }

    public void EquipSkill(SkillSO data)
    {
        if (skills_equipped.Contains(data)) return;
        skills_equipped.Add(data);
    }
    public void UnequipSkill(SkillSO data)
    {
        if (!skills_equipped.Contains(data)) return;
        skills_equipped.Remove(data);
    }

    #endregion

    #region Turn

    public override IEnumerator OnGoingTurn()
    {
        SetRound();
        while (step != TurnStep.Finish)
        {
            switch(step)
            {
                case TurnStep.ChooseSkill:
                    ChoosingSkill();
                    yield return null;
                    break;

                case TurnStep.ChooseTarget:
                    ChoosingTarget();
                    yield return null;
                    break;

                case TurnStep.ExecutingSkill:
                    yield return StartCoroutine(StartSkill());
                    step = TurnStep.Finish;
                    break;
            }
        }
    }

    void ChoosingSkill()
    {
        InputManager.GetInputs();
        if (InputManager.hud_horizontal != 0)
        {
            Debug.Log("Trocou de Skill");
            skill_index = Mathf.Clamp(skill_index + InputManager.hud_horizontal, 0, skills_equipped.Count - 1);
            skill_current = skills_equipped[skill_index];
        }
        if (InputManager.interact && skill_current != null) { step = TurnStep.ChooseTarget; }
    }

    void ChoosingTarget()
    {
        if (!skill_current.chose)
        {
            if (skill_current.affect_enemy)
            {
                foreach(int p in skill_current.pos)
                {
                    if(p < ctx.Enemies.Count) SetTarget(p, ctx.Enemies);
                }
                step = TurnStep.ExecutingSkill; return;
            }

            foreach(int p in skill_current.pos)
            {
                if (p < ctx.Party.Count) SetTarget(p, ctx.Party);
            }
            step = TurnStep.ExecutingSkill; return;
        }
        
        InputManager.GetInputs();
        if (InputManager.interact) step = TurnStep.ExecutingSkill;
        if (!(InputManager.hud_horizontal != 0)) { return; }
        if (skill_current.affect_enemy)
        {
            intent_index = Mathf.Clamp(intent_index + InputManager.hud_horizontal, 0, ctx.Enemies.Count - 1);
            ctx.Target.Clear();
            SetTarget(intent_index, ctx.Enemies);
            return;
        }
            intent_index = Mathf.Clamp(intent_index + InputManager.hud_horizontal, 0, ctx.Party.Count - 1);
            ctx.Target.Clear();
            SetTarget(intent_index, ctx.Party);
        
    }
    public override void SetRound()
    {
        base.SetRound();
        step = TurnStep.ChooseSkill;
        intent_index = 0;
        skill_index = 0;
        skill_current = skills_equipped[skill_index];
    }
        #endregion
}
