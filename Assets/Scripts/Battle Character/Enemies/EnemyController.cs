using NUnit.Framework;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class EnemyController : BattleChar
{
    [SerializeField] protected EnemyBehaviourSO behaviourSO;
    [SerializeField] protected int behaviour_index = 0;
    
    void Start()
    {
    }
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.H)) { StartCoroutine(OnGoingTurn()); }
    }

    #region Init
    public override void InitStats()
    {
        EnemySO so = battle_char as EnemySO;
        base.InitStats();
        life_max = so.life_max;
        life_current = life_max;
        behaviourSO = so.behaviour;
        r_slashing = so.r_slashing;
        r_bludgeoning = so.r_bludgeoning;
        r_pierce = so.r_pierce;
        r_ice = so.r_ice;
        r_fire = so.r_fire;
        r_lightning = so.r_lightning;
        r_purity = so.r_purity;
    }
    #endregion

    #region Turn

    public override IEnumerator OnGoingTurn()
    {
        start_turn?.Invoke(this);
        step = TurnStep.ChooseSkill;

        if(behaviour_index > behaviourSO.behaviour.Count - 1) { behaviour_index = 0; }

        while (step != TurnStep.Finish)
        {
            switch (step)
            {
                case TurnStep.ChooseSkill:
                    ChoosingSkill();
                    yield return null;
                    break;

                case TurnStep.ChooseTarget:
                    {
                        ChoosingTarget();
                        yield return null;
                        break;
                    }

                case TurnStep.ExecutingSkill:
                    yield return StartCoroutine(StartSkill());
                    end_turn?.Invoke(this);
                    behaviour_index++;
                    step = TurnStep.Finish;
                    break;
            }
        }
    }

    void ChoosingSkill()
    {
        /*foreach (ConditionalSkillSO so in behaviourSO.conditional)
                    {
                        if(behaviour_index > behaviourSO.behaviour.Count - 1) { behaviour_index = 0; }
                        if (so.Check())
                        {
                            skill_current = new SkillData(so.skill);
                            step = TurnStep.ChooseTarget;
                            break;
                        }
                    }*/
        skill_current = behaviourSO.behaviour[behaviour_index]; step = TurnStep.ChooseTarget;
    }

    void ChoosingTarget()
    {
        switch(skill_current.target)
        {
            case TypeTarget.Self:
                ctx.Target = ctx.Self;
                break;
            case TypeTarget.Enemy:
                ctx.Target = ctx.RandomChar(ctx.Enemies, skill_current.pos);
                break;
            case TypeTarget.Party:
                ctx.Target = ctx.RandomChar(ctx.Party, skill_current.pos);
                break;
            case TypeTarget.ChooseEnemy:
                ctx.Target = ctx.RandomChar(ctx.Enemies, skill_current.pos);
                break;
            case TypeTarget.ChooseParty:
                ctx.Target = ctx.RandomChar(ctx.Party, skill_current.pos);
                break;
        }
        step = TurnStep.ExecutingSkill;
    }
    #endregion
}
