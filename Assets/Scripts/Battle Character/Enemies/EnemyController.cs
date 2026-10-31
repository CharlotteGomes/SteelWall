using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyController : BattleChar, IPointerEnterHandler
{
    [SerializeField] protected EnemyBehaviourSO behaviourSO;
    [SerializeField] protected int behaviour_index = 0;
    [SerializeField] int original_behaviour = 0;

    public override void InitStats()
    {
        EnemyData data = battle_char as EnemyData;
        if (data == null)
        {
            Debug.LogError($"Enemy data is missing on {name}.", this);
            return;
        }
        behaviourSO = data.behaviour;
        base.InitStats();
    }

    public override void CreateContext(List<BattleChar> allies, List<BattleChar> enemies, int pos)
    {
        base.CreateContext(allies, enemies, pos);
        if (behaviourSO != null && behaviourSO.behaviour != null)
            SetFavoritePos(behaviourSO.behaviour);
    }

    public override void SetFavoritePos(List<SkillSO> so_list)
    {
        base.SetFavoritePos(so_list);
        if (ctx == null) return;

        foreach (FavoritePos favorite in favorite_pos)
        {
            foreach (SkillSO skill in favorite.skills)
            {
                if (skill == null || skill.pos == null) continue;
                List<BattleChar> candidates = skill.affect_enemy ? ctx.Enemies : ctx.Party;
                if (candidates == null) continue;
                foreach (BattleChar candidate in candidates)
                {
                    if (candidate == null || candidate is BattleObjects || !candidate.CheckAlive()) continue;
                    if (skill.pos.Contains(candidate.GetSelfPos())) favorite.AddTarget(candidate);
                }
            }
            favorite.SetValue();
        }
    }

    public override IEnumerator OnGoingTurn()
    {
        while (step != TurnStep.Finish)
        {
            switch (step)
            {
                case TurnStep.ChooseSkill:
                    yield return StartCoroutine(ChoosingSkill());
                    break;
                case TurnStep.ChooseTarget:
                    yield return StartCoroutine(ChoosingTarget());
                    break;
                case TurnStep.ChooseSwapTarget:
                    ChooseSwapTarget();
                    yield return null;
                    break;
                case TurnStep.ExecutingSkill:
                    yield return StartCoroutine(StartSkill());
                    step = TurnStep.Finish;
                    break;
                default:
                    step = TurnStep.Finish;
                    break;
            }
        }
        EndRound();
    }

    private IEnumerator ChoosingSkill()
    {
        if (behaviourSO == null || behaviourSO.behaviour == null || behaviourSO.behaviour.Count == 0)
        {
            step = TurnStep.Finish;
            yield break;
        }
        if (behaviour_index < 0 || behaviour_index >= behaviourSO.behaviour.Count)
            behaviour_index = 0;

        SetSkill(behaviourSO.behaviour[behaviour_index]);
        if (skill_current == null || !CanUseSkill(skill_current))
        {
            BehaviourUpdate();
            yield break;
        }
        yield return StartCoroutine(ConfirmSkill());
    }

    private IEnumerator ConfirmSkill()
    {
        if (skill_current == null)
        {
            BehaviourUpdate();
            yield break;
        }

        skill_current_data?.Dispose();
        skill_current_data = new SkillData(skill_current, ctx);
        if (skill_current_data.targets.Count == 0)
        {
            skill_current_data.Dispose();
            skill_current_data = null;
            BehaviourUpdate();
            yield break;
        }

        OnSkillConfirm?.Invoke(skill_current_data);
        SetStep(TurnStep.ChooseTarget);
        yield return null;
    }

    private IEnumerator ChoosingTarget()
    {
        if (skill_current_data == null || skill_current_data.targets.Count == 0)
        {
            BehaviourUpdate();
            yield break;
        }
        int random = Random.Range(0, skill_current_data.targets.Count);
        SelectTarget(random);
        if (ctx == null || ctx.Target.Count == 0)
        {
            BehaviourUpdate();
            yield break;
        }
        OnBattleCharSelect?.Invoke(ctx.Target.ToArray());
        yield return StartCoroutine(ConfirmTarget());
    }

    private IEnumerator ConfirmTarget()
    {
        yield return new WaitForSeconds(0.75f);
        SetStep(TurnStep.ExecutingSkill);
    }

    public override void ChooseSwapTarget()
    {
        if (favorite_pos.Count == 0 || battle_manager == null || ctx == null)
        {
            step = TurnStep.Finish;
            return;
        }
        FavoritePos best = favorite_pos.OrderByDescending(position => position.value).First();
        int targetPosition = favorite_pos.IndexOf(best);
        if (targetPosition >= 0 && targetPosition < ctx.Party.Count && targetPosition != ctx.SelfPosition)
            battle_manager.SwapPosition(this, targetPosition);
        step = TurnStep.Finish;
    }

    private void BehaviourUpdate()
    {
        if (behaviourSO == null || behaviourSO.behaviour == null || behaviourSO.behaviour.Count == 0)
        {
            step = TurnStep.Finish;
            return;
        }
        behaviour_index = (behaviour_index + 1) % behaviourSO.behaviour.Count;
        if (behaviour_index == original_behaviour)
        {
            SetFavoritePos(behaviourSO.behaviour);
            step = TurnStep.ChooseSwapTarget;
            return;
        }
        step = TurnStep.ChooseSkill;
    }

    public override void SetRound()
    {
        base.SetRound();
        if (behaviourSO == null || behaviourSO.behaviour == null || behaviourSO.behaviour.Count == 0)
        {
            step = TurnStep.Finish;
            return;
        }
        BehaviourUpdate();
        original_behaviour = behaviour_index;
        step = TurnStep.ChooseSkill;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (battle_manager != null && battle_manager.hud != null)
            battle_manager.hud.SetCharacter(this);
    }
}