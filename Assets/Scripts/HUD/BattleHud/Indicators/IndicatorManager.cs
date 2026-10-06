using UnityEngine;
using System.Collections.Generic;
using JetBrains.Annotations;

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
    }

    public void SetTargetIndicators(SkillData data)
    {
        foreach(var c in data.targets)
        {
            Sprite ind_spr = data.original_skill.affect_enemy ? ind_atk : ind_ally;
            Sprite smk_spr = data.original_skill.affect_enemy ? smk_atk : smk_ally;
            if(turn_indicator != null && turn_indicator.character == c) { turn_indicator.SetSprites(ind_spr, smk_spr); continue; }
            GameObject inst = Instantiate(prefab, this.transform);
            Indicator ind = inst.GetComponent<Indicator>();
            ind.SetChar(c);
            ind.SetSprites(ind_spr, smk_spr);
            indicators.Add(ind);
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
        c.OnEndMyTurn += DeleteAllIndicators;
        c.OnEndMyTurn += DeleteTurnIndicator;
    }

    public void UnsubscribeEvents(BattleChar c)
    {
        c.OnStartMyTurn -= SetTurnIndicator;
        c.OnSkillConfirm -= SetTargetIndicators;
        c.OnSkillCancel -= SetTurnIndicatorDefault;
        c.OnSkillCancel -= DeleteAllIndicators;
        c.OnEndMyTurn -= DeleteAllIndicators;
        c.OnEndMyTurn -= DeleteTurnIndicator;
    }

    public void OnDestroy()
    {
        
    }
}
