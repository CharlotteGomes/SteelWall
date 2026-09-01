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
        SetRound();
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
        if (!skill_current.chose)
        {
            if (skill_current.affect_enemy)
            {
                foreach (int p in skill_current.pos)
                {
                    SetTarget(ctx.Enemies[p]);
                }
                step = TurnStep.ExecutingSkill; return;
            }

            foreach (int p in skill_current.pos)
            {
                SetTarget(ctx.Party[p]);
            }
            step = TurnStep.ExecutingSkill; return;
        }

        if(skill_current.affect_enemy)
        {
            int erand = 0;
            foreach (int p in skill_current.pos)
            {
                erand = Random.Range(0, skill_current.pos.Length);
            }
            ctx.Target.Add(ctx.Enemies[erand]);
            step = TurnStep.ExecutingSkill; return;
        }

        int rand = 0;
        foreach (int p in skill_current.pos)
        {
            rand = Random.Range(0, skill_current.pos.Length);
        }
        ctx.Target.Add(ctx.Party[rand]);
        step = TurnStep.ExecutingSkill;
    }
    #endregion

    public override void SetRound()
    {
        base.SetRound();
        behaviour_index = 0;
        step = TurnStep.ChooseSkill;
    }
}
