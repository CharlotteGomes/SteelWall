using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.HID;
public class PjController : BattleChar
{

    [SerializeField] protected List<SkillSO> skills_unlocked = new List<SkillSO>();
    [SerializeField] protected List<SkillSO> skills_equipped = new List<SkillSO>();
    public IReadOnlyList<SkillSO> read_skills => skills_equipped;
    public SkillSO skill_read => skill_current;
    [SerializeField] int intent_index = 0; 
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
        PJSO pj = battle_char as PJSO;
        foreach (SkillSO so in pj.skills)
        {
            AddSkill(so);
        }
        skills_equipped = skills_unlocked;
        //SetFavoritePos(skills_equipped);
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
        EndRound();
    }

    protected void ChoosingSkill()
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
        if (index >= skills_equipped.Count) { SetStep(TurnStep.MoveAction); OnMoveActionSelect?.Invoke(); return;  }
        skill_current = skills_equipped[index];
        OnSkillSelect?.Invoke(skill_current);
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
                skill_current_data = new SkillData(skill_current, ctx);
                intent_index = 0;
                OnSkillConfirm?.Invoke(skill_current_data);
                SelectTarget(intent_index);
                break;
            }
        }
        
    }
    protected void ChoosingTarget()
    {
        InputManager.GetInputs();
        if(InputManager.esc || InputManager.back) { step = TurnStep.ChooseSkill; ctx.Target.Clear(); skill_current_data = null; OnSkillCancel?.Invoke(); return; }
        if (InputManager.hud_horizontal != 0)
        {
            if (skill_current.affect_enemy) {SelectTarget(intent_index + InputManager.hud_horizontal); return;}
            SelectTarget(intent_index - InputManager.hud_horizontal);
        }
        if (InputManager.interact) ConfirmTarget();
    }

    public override void SelectTarget(int index)
    {
        base.SelectTarget(index);
        if (index < 0 || index >= skill_current_data.targets.Count) return;
        intent_index = index;
        OnBattleCharSelect?.Invoke(ctx.Target.ToArray());

        Debug.Log(ctx.Target.ToArray().Length);
    }
    public void ConfirmTarget() 
    {
        if (skill_current == null) return;
        if(ctx.Target.Count == 0) return;
        step = TurnStep.ExecutingSkill;
    }

    protected void MoveAction() 
    {
        InputManager.GetInputs();
        if (InputManager.hud_horizontal == 1)
        {
            OnSkipSelect?.Invoke();
            SetStep(TurnStep.SkipTurn);
        }
        if(InputManager.hud_horizontal == -1)
        {
            SelectSkill(3);
            SetStep(TurnStep.ChooseSkill);
        }
        if (InputManager.interact) { ConfirmMoveAction();  }
    }

    public void ConfirmMoveAction()
    {
        intent_index = ctx.SelfPosition; step = TurnStep.ChooseSwapTarget;
        OnMoveConfirm?.Invoke(ctx);
    }

    public override void ChooseSwapTarget()
    {
        InputManager.GetInputs();
        if (InputManager.esc || InputManager.back) { OnMoveCancel?.Invoke(); step = TurnStep.MoveAction; return; }
        if (InputManager.hud_horizontal != 0)
        {

            int new_intent = Mathf.Abs(InputManager.hud_horizontal - intent_index) - ctx.SelfPosition > move  ? intent_index : intent_index - InputManager.hud_horizontal ;
        }

        if (InputManager.interact) { ConfirmSwapTarget(intent_index); }
    }
    // 0, 1, eu, 3
    void SelectSwapTarget(int i)
    {
        // 0 1 3
        List<BattleChar> l = ctx.Party.Where(x => x != ctx.Self).ToList();
    } 

    public void SkipTurn()
    {
        InputManager.GetInputs();
        if (InputManager.hud_horizontal == -1)
        {
            OnMoveActionSelect?.Invoke();
            SetStep(TurnStep.MoveAction);
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

    public override void SetRound()
    {
        base.SetRound();
        step = TurnStep.ChooseSkill;
        SelectSkill(0);
    }
        #endregion
}
