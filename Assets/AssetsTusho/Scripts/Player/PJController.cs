using UnityEngine;
using System;
using System.Collections;

public enum TurnPhase
{
    ChoosingAction,
    ChoosingSkill,
    ChoosingTarget,
    Finished
}

public class PJController : BattleCharacter, IDamageable, IKillable
{

    public int targetIdx = 0;
    public int skillIdx = 0;
    public bool CanControl;
    [SerializeField] SkillBase currentSkill;
    public override IEnumerator ExecuteTurn()
    {
        CanControl = true;
        targetIdx = 0;
        skillIdx = 0;
        ctx.target = ctx.enemies[0];
        ChangePhase(TurnPhase.ChoosingSkill);
        while (phase != TurnPhase.Finished)
        {
            switch (phase)
            {
                case TurnPhase.ChoosingSkill:
                    ChoosingSkill();
                    break;

                case TurnPhase.ChoosingTarget:
                    ChoosingTarget();
                    if (InputHandler.Instance.ConfirmAction) { yield return StartCoroutine(currentSkill.Execute(ctx)); ChangePhase(TurnPhase.Finished); }
                    break;

                case TurnPhase.Finished:
                    break;
            }
            yield return null;
        }
    }

    void ChoosingSkill()
    {
        ChangeSkill(InputHandler.Instance.HUDInput.y);
        currentSkill = SkillList[skillIdx];
        if (InputHandler.Instance.ConfirmAction) { ChangePhase(TurnPhase.ChoosingTarget); }
    }

    void ChoosingTarget()
    {
        if (InputHandler.Instance.HUDInput.y != 0) { ChangeTarget(InputHandler.Instance.HUDInput.y); }
        if (InputHandler.Instance.CancelAction) { ChangePhase(TurnPhase.ChoosingSkill); }
    }

    void ChangeTarget(int idx)
    {
        if(currentSkill.tTarget == TargetType.Enemies)
        {
            targetIdx = Mathf.Clamp(targetIdx -= idx, 0, ctx.enemies.Count - 1);
            ctx.target = ctx.enemies[targetIdx];

        }
        if (currentSkill.tTarget == TargetType.Allies)
        {
            targetIdx = Mathf.Clamp(targetIdx -= idx, 0, ctx.allies.Count - 1);
            ctx.target = ctx.allies[targetIdx];
        }
        OnTargetChanged?.Invoke(ctx.target);
    }

    void ChangeSkill(int idx)
    {
        skillIdx = Mathf.Clamp(skillIdx - idx, 0, SkillList.Count - 1);
        OnSkillChanged?.Invoke(idx);
    }

    void ChangePhase(TurnPhase newphase)
    {
        phase = newphase;
        OnPhaseChanged?.Invoke(newphase, this);
    }
    void Start()
    {
        InitializeStats();
    }
    
    void Update()
    {
    }
}
