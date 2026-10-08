using UnityEngine;
using System.Collections.Generic;
using JetBrains.Annotations;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
public class IndicatorManager : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] List<Indicator> indicators = new List<Indicator>();
    [SerializeField] Indicator turn_indicator = null;
    [SerializeField] Sprite ind_turn, ind_atk, ind_move, ind_ally, smk_atk, smk_move, smk_ally;
    [SerializeField] BattleChar test;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            SetTurnIndicator(test);
        }
    }

    public void SetTurnIndicator(BattleChar c) 
    {
        GameObject inst = Instantiate(prefab, this.transform);
        Indicator ind = inst.GetComponent<Indicator>();
        turn_indicator = ind;
        ind.SetChar(c);
        ind.SetSprites(ind_turn, null);
        StartCoroutine(ind.StartIndAnimation(c));
    }

    public void SetTurnIndicatorDefault()
    {
        turn_indicator.SetSprites(ind_turn, null);
        turn_indicator.Select(true);
    }

    public void SetTargetIndicators(SkillData data)
    {
        foreach(var c in data.targets)
        {
            Sprite ind_spr = data.original_skill.affect_enemy ? ind_atk : ind_ally;
            Sprite smk_spr = data.original_skill.affect_enemy ? smk_atk : smk_ally;
            if(turn_indicator != null && turn_indicator.character == c) { turn_indicator.SetSprites(ind_spr, smk_spr); turn_indicator.Select(false); continue; }
            GameObject inst = Instantiate(prefab, this.transform);
            Indicator ind = inst.GetComponent<Indicator>();
            ind.SetChar(c);
            ind.SetSprites(ind_spr, smk_spr);
            ind.Select(false);
            indicators.Add(ind);
        }
    }
    public void SelectIndicators(BattleChar[] c)
    {
        for (int i = 0; i < c.Length; i++)
        {
            if (turn_indicator.character == c[i]) { turn_indicator.Select(true); }
            else turn_indicator.Select(false);
                for (int j = 0; j < indicators.Count; j++)
                {
                    if (indicators[j].character == c[i]) { indicators[j].Select(true); continue; }
                    indicators[j].Select(false);
                }
        }
    }

    public void SetMoveIndicators(BattleContext ctx) 
    {
        for(int i = -ctx.Self.GetStat(StatType.MOVE); i <= ctx.Self.GetStat(StatType.MOVE); i++)
        {
            
            int pos = i + ctx.SelfPosition;
            Debug.Log($"{i} {pos}");
            if (pos < 0 || pos >= ctx.Party.Count) continue;
            if (ctx.Party[pos] == null || ctx.Party[pos] == ctx.Self) continue;
            GameObject inst = Instantiate(prefab, this.transform);
            Indicator ind = inst.GetComponent<Indicator>();
            ind.SetChar(ctx.Party[pos]);
            ind.SetSprites(ind_move, smk_move);
            indicators.Add(ind);
        }
    }

    public void SelectMoveIndicators(BattleChar c)
    {
        foreach(Indicator indicator in indicators)
        {
            if (indicator.character == c)
            {
                indicator.Select(true);
                continue;
            }
            indicator.Select(false);
        }
    }

    public void DeleteAllIndicators()
    {
        foreach(Indicator ind in indicators)
        {
            Destroy(ind.gameObject);
        }
        indicators.Clear();
    }

    public void DeleteTurnIndicator()
    {
        Destroy(turn_indicator.gameObject);
        turn_indicator = null;
    }
    
    public void SubscribeEvents(BattleChar c)
    {
        c.OnStartMyTurn += SetTurnIndicator;
        c.OnSkillConfirm += SetTargetIndicators;
        c.OnSkillCancel += SetTurnIndicatorDefault;
        c.OnSkillCancel += DeleteAllIndicators;
        c.OnMoveCancel += DeleteAllIndicators;
        c.OnBattleCharSelect += SelectIndicators;
        c.OnMoveConfirm += SetMoveIndicators;
        c.OnSwapTargetSelect += SelectMoveIndicators;
        c.OnEndMyTurn += DeleteAllIndicators;
        c.OnEndMyTurn += DeleteTurnIndicator;
    }

    public void UnsubscribeEvents(BattleChar c)
    {
        c.OnStartMyTurn -= SetTurnIndicator;
        c.OnSkillConfirm -= SetTargetIndicators;
        c.OnSkillCancel -= SetTurnIndicatorDefault;
        c.OnSkillCancel -= DeleteAllIndicators;
        c.OnMoveCancel -= DeleteAllIndicators;
        c.OnBattleCharSelect -= SelectIndicators;
        c.OnMoveConfirm -= SetMoveIndicators;
        c.OnSwapTargetSelect -= SelectMoveIndicators;
        c.OnEndMyTurn -= DeleteAllIndicators;
        c.OnEndMyTurn -= DeleteTurnIndicator;
    }

    public void OnDestroy()
    {
        
    }
}
