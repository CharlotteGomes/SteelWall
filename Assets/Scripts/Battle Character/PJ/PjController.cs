using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;
public class PjController : BattleChar
{
    [SerializeField] List<SkillSO> test = new List<SkillSO>();

    [SerializeField] protected List<SkillSO> skills_unlocked = new List<SkillSO>();
    [SerializeField] protected List<SkillSO> skills_equipped = new List<SkillSO>();
    public IReadOnlyList<SkillSO> read_skills => skills_equipped;

    public int action_index { get; private set; } = 0;
    public int intent_index { get; private set; } = 0;
    protected override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Init
    public override void InitStats()
    {
        foreach (SkillSO wo in test)
        {
            AddSkill(wo);
        }
        skills_equipped = skills_unlocked;
        SetFavoritePos(skills_equipped);
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

                case TurnStep.MoveAction:
                    MoveAction();
                    if (action_index <= 3) { step = TurnStep.ChooseSkill; SelectSkill(action_index);}
                    if (action_index >= 5) step = TurnStep.SkipTurn;
                    yield return null;
                    break;

                case TurnStep.ChooseSwapTarget:
                    ChooseSwapTarget();
                    yield return null;
                    break;

                case TurnStep.ExecutingSkill:
                    yield return StartCoroutine(StartSkill());
                    step = TurnStep.Finish;
                    break;
                case TurnStep.SkipTurn:
                    step = TurnStep.Finish;
                    break;
            }
        }
    }

    protected override void ChoosingSkill()
    {
        InputManager.GetInputs();
        if (InputManager.hud_horizontal != 0)
        {
            SelectSkill(action_index + InputManager.hud_horizontal);
        }
        if (InputManager.interact){ConfirmSkill();}
    }

    public void SelectSkill(int index)
    {
        if (index < 0) { return; }
        SetActionIndex(index);
        if (index >= skills_equipped.Count) {step = TurnStep.MoveAction; battle_manager.hud.pj_panel.DiselectAllSkillIcon(); return; }
        SetSkill(skills_equipped[action_index]);
        battle_manager.hud.pj_panel.SelectSkillIcon(action_index);
    }

    public void ConfirmSkill()
    {
        if (skill_current == null)
            return;

        for (int i = 0; i < skill_current.use_pos.Length; i++)
        {
            if (ctx.SelfPosition == skill_current.use_pos[i])
            {
                step = TurnStep.ChooseTarget;
                SelectTarget(0);
                return;
            }
        }
    }
    protected override void ChoosingTarget()
    {
        InputManager.GetInputs();
        if(InputManager.esc || InputManager.back) { step = TurnStep.ChooseSkill; ctx.Target.Clear(); return; }
        if (InputManager.hud_horizontal != 0) SelectTarget(intent_index + InputManager.hud_horizontal);
        if (InputManager.interact) ConfirmTarget();
    }

    public override void SelectTarget(int index)
    {
        if(skill_current == null) { return; }
        if (index < 0 || index >= ctx.Enemies.Count) return;
        if (index < skill_current.pos[0] || index > skill_current.pos[skill_current.pos.Length - 1])
        {
            step = TurnStep.ChooseSkill;
            return;
        }
        
        intent_index = index;
        ctx.Target.Clear();
        if (!skill_current.chose)
        {
            if (skill_current.affect_enemy)
            {
                foreach (int p in skill_current.pos)
                {
                    if (p < ctx.Enemies.Count) SetTarget(p, ctx.Enemies);
                }
                return;
            }

            foreach (int p in skill_current.pos)
            {
                if (p < ctx.Party.Count) SetTarget(p, ctx.Party);
            }
            return;
        }
        
        if (skill_current.affect_enemy)
        {
            ctx.Target.Clear();
            SetTarget(intent_index, ctx.Enemies);
            return;
        }
        ctx.Target.Clear();
        SetTarget(intent_index, ctx.Party);
    }

    public override void ConfirmTarget() 
    {
        if (skill_current == null) return;
        if(ctx.Target.Count == 0) return;
        step = TurnStep.ExecutingSkill;
    }

    protected void MoveAction() 
    {
        InputManager.GetInputs();
        if (InputManager.hud_horizontal != 0)
        {
            SetActionIndex(action_index + InputManager.hud_horizontal);
        }
        if (InputManager.interact) { intent_index = ctx.SelfPosition;  step = TurnStep.ChooseSwapTarget; }
    }
    public override void ChooseSwapTarget()
    {
        InputManager.GetInputs();
        if (InputManager.esc || InputManager.back) { step = TurnStep.ChooseSkill; return; }
        if (InputManager.hud_horizontal != 0)
        {
            int new_intent = InputManager.hud_horizontal == 1 ? ctx.SelfPosition - 1 : ctx.SelfPosition + 1;
            intent_index = (new_intent < 0 || new_intent > ctx.Party.Count - 1) ? ctx.SelfPosition : new_intent;
        }

        if (InputManager.interact) { ConfirmSwapTarget(intent_index); }
    }

    public void SetActionIndex(int index)
    {
        if(index < 0 || index > 5) return;
        action_index = index;
    }

    public override void SetRound()
    {
        base.SetRound();
        step = TurnStep.ChooseSkill;
        intent_index = 0;
        action_index = 0;
        skill_current = skills_equipped[action_index];
    }
        #endregion
}
