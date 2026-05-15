using UnityEngine;
using System;
using System.Collections;

public enum PlayerTurnPhase
{
    ChoosingAction,
    ChoosingSkill,
    ChoosingTarget,
    Finished
}

public class PJController : BattleCharacter, IDamageable, IKillable
{
    PlayerTurnPhase phase;
    [SerializeField] int targetIdx = 0;
    [SerializeField] int skillIdx = 0;
    [SerializeField] SkillBase currentSkill;
    public override IEnumerator ExecuteTurn()
    {
        phase = PlayerTurnPhase.ChoosingSkill;
        targetIdx = 0;
        skillIdx = 0;
        while(phase != PlayerTurnPhase.Finished)
        {
            switch (phase)
            {
                case PlayerTurnPhase.ChoosingSkill:
                    skillIdx = Mathf.Clamp(skillIdx += InputHandler.Instance.HUDInput.y, 0, SkillList.Count - 1);
                    if (InputHandler.Instance.ConfirmAction) { currentSkill = SkillList[skillIdx]; phase = PlayerTurnPhase.ChoosingTarget; }
                    break;
                case PlayerTurnPhase.ChoosingTarget:
                    if(currentSkill.tTarget == TargetType.Enemies) { targetIdx = Mathf.Clamp(targetIdx += InputHandler.Instance.HUDInput.y, 0, ctx.enemies.Count - 1); ctx.target = ctx.enemies[targetIdx]; }
                    else { targetIdx = Mathf.Clamp(targetIdx += InputHandler.Instance.HUDInput.y, 0, ctx.allies.Count - 1); ctx.target = ctx.allies[targetIdx]; }

                    if (InputHandler.Instance.ConfirmAction) {  yield return StartCoroutine(currentSkill.Execute(ctx)); phase = PlayerTurnPhase.Finished; }
                    break;
                case PlayerTurnPhase.Finished:
                    break;
            }
            yield return null;
        }
    }

    #region native
    void OnEnable()
    {
        
    }

    void OnDisable()
    {
        
    }

    void Start()
    {
        InitializeStats();
    }
    
    void Update()
    {
        
    }
    #endregion
}
