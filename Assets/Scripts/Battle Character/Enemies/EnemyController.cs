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
    }

    #region Init
    public override void InitStats()
    {
        EnemySO so = battle_char as EnemySO;
        base.InitStats();
        life_current = life_max;
        behaviourSO = so.behaviour;
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
        skill_current = behaviourSO.behaviour[behaviour_index]; 
        for (int i = 0; i < skill_current.use_pos.Length; i++)
        {
            if (ctx.SelfPosition == skill_current.use_pos[i]) { step = TurnStep.ChooseTarget; return; }
        }
        BehaviourUpdate();
    }

    void ChoosingTarget()
    {
        if (!skill_current.chose)
        {
            if (skill_current.affect_enemy)
            {
                foreach (int p in skill_current.pos)
                {
                    if(p < ctx.Enemies.Count) SetTarget(p, ctx.Enemies);
                }
                if (ctx.Target.Count == 0) { BehaviourUpdate(); return; }
                step = TurnStep.ExecutingSkill; return;
            }

            foreach (int p in skill_current.pos)
            {
                if (p < ctx.Party.Count) SetTarget(p, ctx.Party);
            }
            if (ctx.Target.Count == 0) { BehaviourUpdate(); return; }
            step = TurnStep.ExecutingSkill; return;
        }

        if(skill_current.affect_enemy)
        {
            List<BattleChar> elist = new List<BattleChar>();
            foreach (int p in skill_current.pos)
            {
                if(p < ctx.Enemies.Count) { elist.Add(ctx.Enemies[p]); }
            }
            int erand = Random.Range(0, elist.Count);
            SetTarget(erand, ctx.Enemies);
            if (ctx.Target.Count == 0) { BehaviourUpdate(); return; }
            step = TurnStep.ExecutingSkill; return;
        }

        List<BattleChar> list = new List<BattleChar>();
        foreach (int p in skill_current.pos)
        {
            if (p < ctx.Party.Count) { list.Add(ctx.Party[p]); }
        }
        int rand = Random.Range(0, list.Count);
        SetTarget(rand, ctx.Party);
        if(ctx.Target.Count == 0) { BehaviourUpdate(); return; }
        step = TurnStep.ExecutingSkill;
    }

    void BehaviourUpdate()
    {
        behaviour_index++;
        if(behaviour_index > behaviourSO.behaviour.Count - 1) { behaviour_index = 0; }
        step = TurnStep.ChooseSkill;
    }
    #endregion

    public override void SetRound()
    {
        base.SetRound();
        step = TurnStep.ChooseSkill;
    }
}
