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
                    ChoosingTarget();
                    yield return null;
                    break;

                case TurnStep.ChooseSwapTarget:
                    ChooseSwapTarget();
                    yield return null;
                    break;

                case TurnStep.ExecutingSkill:
                    yield return StartCoroutine(StartSkill());
                    behaviour_index++;
                    step = TurnStep.Finish;
                    break;
            }
        }
    }

    protected override void ChoosingSkill()
    {
        skill_current = behaviourSO.behaviour[behaviour_index]; 
        for (int i = 0; i < skill_current.use_pos.Length; i++)
        {
            if (ctx.SelfPosition == skill_current.use_pos[i]) { step = TurnStep.ChooseTarget; return; }
        }
        BehaviourUpdate();
    }

    protected override void ChoosingTarget()
    {
        List<int> range = new List<int>();
        
        for (int i = 0; i < skill_current.pos.Length; i++)
        {
            if (skill_current.affect_enemy)
            {
                if (skill_current.pos[i] < ctx.Enemies.Count) { range.Add(skill_current.pos[i]); continue; }
            }

            if (!skill_current.affect_enemy)
            {
                if (skill_current.pos[i] < ctx.Party.Count) { range.Add(skill_current.pos[i]); continue; }
            }
        }

        int random = Random.Range(0, range.Count);

        if (random < range.Count)
        {
            SelectTarget(range[random]);
            ConfirmTarget();
            return;
        }
        BehaviourUpdate();
    }

    public override void SelectTarget(int index)
    {
        if (skill_current == null) { return; }
        if (index < 0 || index >= ctx.Enemies.Count) return;
        ctx.Target.Clear();
        if (!skill_current.chose)
        {
            if (skill_current.affect_enemy)
            {
                foreach (int p in skill_current.pos)
                {
                    if (p < ctx.Enemies.Count) SetTarget(p, ctx.Enemies);
                }
                return;
            }

            foreach (int p in skill_current.pos)
            {
                if (p < ctx.Party.Count) SetTarget(p, ctx.Party);
            }
            return;
        }

        if (skill_current.affect_enemy)
        {
            ctx.Target.Clear();
            SetTarget(index, ctx.Enemies);
            return;
        }
        ctx.Target.Clear();
        SetTarget(index, ctx.Party);
    }

    public override void ConfirmTarget()
    {
        Debug.Log("Entrou no confirm");
        if (skill_current == null) return;
        if (ctx.Target.Count == 0) { Debug.Log("Sem alvos"); return; }
        step = TurnStep.ExecutingSkill;
    }
    public override void ChooseSwapTarget() 
    {
        List<FavoritePos> mostvalue = favorite_pos.OrderByDescending(a => a.value).ToList();
        int mostvaluepos = 0;

        for (int i = 0; i < mostvalue.Count; i++) 
        {
            if (favorite_pos[i] == mostvalue[0]) mostvaluepos = i;   
        }

        int dir = mostvaluepos - ctx.SelfPosition;
        int intent = ctx.SelfPosition + (int)Mathf.Sign(dir);
        battle_manager.SwapPosition(this, ctx.Party[intent]);
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

    public override void SetRound()
    {
        base.SetRound();
        original_behaviour = behaviour_index;
        step = TurnStep.ChooseSkill;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        battle_manager.hud.SetCharacterEnemy(this);
    }
}
