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
        behaviourSO = so.behaviour;
        base.InitStats();
        life_current = life_max;
        SetFavoritePos(behaviourSO.behaviour);
    }
    public override void SetFavoritePos(List<SkillSO> so_list)
    {
        base.SetFavoritePos(so_list);
        
        foreach(FavoritePos fp in favorite_pos)
        {
            foreach(SkillSO s in fp.skills)
            {
                List<BattleChar> b = new();
                b = s.affect_enemy ? ctx.Enemies : ctx.Party; 
                for(int i = 0; i < s.pos.Length; i++)
                {
                    if (i >= b.Count) continue;
                    fp.AddTarget(b[i]);
                }
            }
            fp.SetValue();
        }
    }
    #endregion

    #region Turn

    public override IEnumerator OnGoingTurn()
    {
        while (step != TurnStep.Finish)
        {
            switch (step)
            {
                case TurnStep.ChooseSkill:
                    StartCoroutine(ChoosingSkill());
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
            }
        }
        OnEndMyTurn?.Invoke();
    }

    IEnumerator ChoosingSkill()
    {
        SetSkill(behaviourSO.behaviour[behaviour_index]);
        if(!CanUseSkill(skill_current))
        {
            BehaviourUpdate();
            yield break;
        }
        yield return StartCoroutine(ConfirmSkill());
    }

    IEnumerator ConfirmSkill()
    {
        if(skill_current == null) { BehaviourUpdate(); yield break; }
        skill_current_data = new SkillData(skill_current, ctx);
        if(skill_current_data.targets.Count <= 0) { BehaviourUpdate(); yield break; }
        OnSkillConfirm?.Invoke(skill_current_data);
        SetStep(TurnStep.ChooseTarget);
    }

    IEnumerator ChoosingTarget()
    {
        int random = Random.Range(0, skill_current_data.targets.Count);
        SelectTarget(random);
        if(ctx.Target.Count <= 0) { BehaviourUpdate(); yield break; }
        OnBattleCharSelect?.Invoke(ctx.Target.ToArray());
        yield return StartCoroutine(ConfirmTarget());
    }

    IEnumerator ConfirmTarget()
    {
        yield return new WaitForSeconds(0.75f);
        SetStep(TurnStep.ExecutingSkill);
    }

    /*
    protected void ChoosingSkill()
    {
        skill_current = behaviourSO.behaviour[behaviour_index]; 
        
        Debug.Log("Choosing skill");
        BehaviourUpdate();
    }

    protected IEnumerator ChooseTarget()
    {
        skill_current_data = new SkillData(skill_current, ctx);
        if (skill_current_data == null || skill_current_data.targets.Count == 0) { BehaviourUpdate(); yield break; }  
        int random = Random.Range(0, skill_current_data.targets.Count);
        SelectTarget(random);
        yield return StartCoroutine(ConfirmTarget());
    }

    public IEnumerator ConfirmTarget()
    {
        Debug.Log("Entrou no confirm");
        if (skill_current == null) yield break;
        if (ctx.Target.Count == 0) { Debug.Log("Sem alvos"); yield break; }
        yield return StartCoroutine(ConfirmSkill());
    }

    public IEnumerator ConfirmSkill()
    {
        if (skill_current == null) { yield break; }
        Debug.Log("Confirmou a skill");
        OnSkillConfirm?.Invoke(skill_current_data);
        yield return new WaitForSeconds(.75f);
        step = TurnStep.ExecutingSkill;
    }*/

    public override void ChooseSwapTarget() 
    {
        List<FavoritePos> mostvalue = favorite_pos.OrderByDescending(a => a.value).ToList();
        int mostvaluepos = 0;

        for (int i = 0; i < mostvalue.Count; i++) 
        {
            if (favorite_pos[i] == mostvalue[0]) mostvaluepos = i;   
        }

        int dir = mostvaluepos - ctx.SelfPosition;
        int intent = ctx.SelfPosition + dir;
        Debug.Log($"{name} intend: {intent}");
        battle_manager.SwapPosition(this, intent);
        step = TurnStep.Finish;
    }

    void BehaviourUpdate()
    {
        behaviour_index++;
        if (behaviour_index > behaviourSO.behaviour.Count - 1) { behaviour_index = 0; }
        if (behaviour_index == original_behaviour)
        {
            SetFavoritePos(behaviourSO.behaviour);
            step = TurnStep.ChooseSwapTarget;
            return;
        }
        step = TurnStep.ChooseSkill;
    }
    #endregion

    #region info 
    #endregion

    public override void SetRound()
    {
        base.SetRound();
        BehaviourUpdate();
        original_behaviour = behaviour_index;
        step = TurnStep.ChooseSkill;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        battle_manager.hud.SetCharacter(this);
    }
}
