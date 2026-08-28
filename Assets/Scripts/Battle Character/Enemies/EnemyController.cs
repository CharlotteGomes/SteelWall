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

        while(step != TurnStep.Finish)
        {
            switch (step)
            {
                case TurnStep.ChooseSkill:
                    ChoosingSkill();
                    break;

                case TurnStep.ChooseTarget:
                    {
                        ChoosingTarget();
                        break;
                    }

                case TurnStep.ExecutingSkill:
                    yield return StartCoroutine(StartSkill());
                    end_turn?.Invoke(this);
                    behaviour_index++;
                    step = TurnStep.Finish;
                    break;
            }
            yield return null;
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
        skill_current = new SkillData(behaviourSO.behaviour[behaviour_index]); step = TurnStep.ChooseTarget;
    }

    void ChoosingTarget()
    {
        switch (skill_current.skill.target)
        {
            case TypeTarget.Self:
                ctx.Target = ctx.Self; break;

            case TypeTarget.EnemyTarget:
                int e_rand = Random.Range(0, ctx.Enemies.Count);
                ctx.Target = ctx.Enemies[e_rand]; break;

            case TypeTarget.PartyTarget:
                int p_rand = Random.Range(0, ctx.Party.Count);
                ctx.Target = ctx.Party[p_rand]; break;

            case TypeTarget.OtherParty:
                List<BattleChar> p = ctx.Party.Where(n => n != this).ToList();
                if (p.Count == 0)
                {
                    behaviour_index++;
                    ctx.Target = null;
                    step = TurnStep.ChooseSkill; break;
                }
                int op_rand = Random.Range(0, p.Count);
                ctx.Target = p[op_rand]; break;
        }
        step = TurnStep.ExecutingSkill;
    }
    #endregion
}
