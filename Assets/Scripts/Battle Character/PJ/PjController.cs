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
    public SkillSO skill_read => skill_current;
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
                    yield return null;
                    break;

                case TurnStep.ChooseSwapTarget:
                    ChooseSwapTarget();
                    yield return null;
                    break;

                case TurnStep.SkipTurn:
                    SkipTurn();
                    yield return null;
                    break;

                case TurnStep.ExecutingSkill:
                    yield return StartCoroutine(StartSkill());
                    step = TurnStep.Finish;
                    break;
                
            }
        }
    }

    public void SelectChooseSkill()
    {
        SetStep(TurnStep.ChooseSkill);
        battle_manager.hud.pj_hud.SelectSkill(skills_equipped.IndexOf(skill_current));
        battle_manager.hud.pj_hud.SelectSkipButton(false);
    }

    protected override void ChoosingSkill()
    {
        InputManager.GetInputs();
        if (InputManager.hud_horizontal != 0)
        {
            int new_index = FindSkillIndex(skill_current) + InputManager.hud_horizontal;
            SelectSkill(new_index);
        }
        if (InputManager.interact){ConfirmSkill();}
    }

    public void SelectSkill(int index)
    {
        if (index < 0) { return; }
        if(index >= skills_equipped.Count) { SelectMoveAction(); return; }
        skill_current = skills_equipped[index];
        battle_manager.hud.pj_hud.SelectSkill(index);
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

    public void SelectMoveAction()
    {
        SetStep(TurnStep.MoveAction);
        battle_manager.hud.pj_hud.SelectSkill();
        battle_manager.hud.pj_hud.SelectMoveButton();
        battle_manager.hud.pj_hud.SelectSkipButton(false);
    }

    protected void MoveAction() 
    {
        InputManager.GetInputs();
        if (InputManager.hud_horizontal == 1)
        {
            SelectSkipTurn();
        }
        if(InputManager.hud_horizontal == -1)
        {
            SelectSkill(3);
            SelectChooseSkill();
        }
        if (InputManager.interact) { ConfirmMoveAction();  }
    }

    public void ConfirmMoveAction()
    {
        intent_index = ctx.SelfPosition; step = TurnStep.ChooseSwapTarget;
    }

    public override void ChooseSwapTarget()
    {
        InputManager.GetInputs();
        if (InputManager.esc || InputManager.back) { step = TurnStep.MoveAction; return; }
        if (InputManager.hud_horizontal != 0)
        {
            int new_intent = InputManager.hud_horizontal == 1 ? ctx.SelfPosition - 1 : ctx.SelfPosition + 1;
            SetIntentIndex(new_intent, ctx.Party);
        }

        if (InputManager.interact) { ConfirmSwapTarget(intent_index); }
    }

    public void SelectSkipTurn()
    {
        SetStep(TurnStep.SkipTurn);
        battle_manager.hud.pj_hud.SelectSkill();
        battle_manager.hud.pj_hud.SelectSkipButton();
    }

    public void SkipTurn()
    {
        InputManager.GetInputs();
        if (InputManager.hud_horizontal == -1)
        {
            SelectMoveAction();
        }
        if (InputManager.interact) { ConfirmSkip(); }
    }

    public void ConfirmSkip()
    {
        step = TurnStep.Finish; 
    }

    public int FindSkillIndex(SkillSO so)
    {
        return skills_equipped.IndexOf(so);
    }

    public void SetIntentIndex(int index, List<BattleChar> list)
    {
        if (index < 0 || index >= list.Count) return;
        intent_index = index;   
    }

    public override void SetRound()
    {
        base.SetRound();
        step = TurnStep.ChooseSkill;
        SelectSkill(0);
    }
        #endregion
}
