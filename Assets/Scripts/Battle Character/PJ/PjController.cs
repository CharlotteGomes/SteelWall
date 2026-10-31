using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class PjController : BattleChar
{

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
        PJData pj = battle_char as PJData;
        skills_equipped = pj.SkillEquipped.ToList();
        base.InitStats();
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
        if (skill_current == null || skill_current.use_pos == null || ctx == null)
            return;

        for (int i = 0; i < skill_current.use_pos.Length; i++)
        {
            if (ctx.SelfPosition == skill_current.use_pos[i])
            {
                skill_current_data?.Dispose();
                skill_current_data = new SkillData(skill_current, ctx);
                if (skill_current_data.targets.Count == 0)
                {
                    skill_current_data.Dispose();
                    skill_current_data = null;
                    ctx.Target.Clear();
                    step = TurnStep.ChooseSkill;
                    return;
                }
                step = TurnStep.ChooseTarget;
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
        if (InputManager.esc || InputManager.back)
        {
            step = TurnStep.ChooseSkill;
            ctx.Target.Clear();
            skill_current_data?.Dispose();
            skill_current_data = null;
            OnSkillCancel?.Invoke();
            return;
        }
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
    }
    public void ConfirmTarget() 
    {
        if (skill_current == null || skill_current_data == null || ctx == null) return;
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
        SelectSwapTarget(ctx.SelfPosition, 1);
    }

    public override void ChooseSwapTarget()
    {
        InputManager.GetInputs();
        if (InputManager.esc || InputManager.back) { OnMoveCancel?.Invoke(); step = TurnStep.MoveAction; return; }
        if (InputManager.hud_horizontal != 0)
        {
            int new_intent = intent_index - InputManager.hud_horizontal;
            if (Mathf.Abs(new_intent - ctx.SelfPosition) > GetStat(StatType.MOVE)) return;
            int dir = (int)Mathf.Sign(new_intent - intent_index);
            SelectSwapTarget(new_intent, dir);
        }

        if (InputManager.interact) { ConfirmSwapTarget(intent_index); }
    }
    void SelectSwapTarget(int i, int dir)
    {
        if (ctx == null || ctx.Party == null || i < 0 || i >= ctx.Party.Count) return;
        if (i == ctx.SelfPosition)
        {
            if (i + dir >= 0 && i + dir < ctx.Party.Count)
            {
                intent_index = i + dir;
                Debug.Log("Passou direto");
                OnSwapTargetSelect?.Invoke(ctx.Party[intent_index]);
                return;
            }
            return;
        }
        intent_index = i;
        OnSwapTargetSelect?.Invoke(ctx.Party[intent_index]);
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
