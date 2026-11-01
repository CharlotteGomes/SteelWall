using System.Collections.Generic;
using UnityEngine;

public class IndicatorManager : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private List<Indicator> indicators = new List<Indicator>();
    [SerializeField] private Indicator turn_indicator;
    [SerializeField] private Sprite ind_turn, ind_atk, ind_move, ind_ally, smk_atk, smk_move, smk_ally;

    private readonly HashSet<BattleChar> subscribedCharacters = new HashSet<BattleChar>();

    public void SetTurnIndicator(BattleChar character)
    {
        if (character == null || prefab == null) return;

        DeleteTurnIndicator();
        GameObject instance = Instantiate(prefab, transform);
        turn_indicator = instance.GetComponent<Indicator>();
        if (turn_indicator == null)
        {
            Destroy(instance);
            return;
        }

        turn_indicator.SetChar(character);
        turn_indicator.SetSprites(ind_turn, null);
        StartCoroutine(turn_indicator.StartIndAnimation(character));
    }

    public void SetTurnIndicatorDefault()
    {
        if (turn_indicator == null) return;
        turn_indicator.SetSprites(ind_turn, null);
        turn_indicator.Select(true);
    }

    public void SetTargetIndicators(SkillData data)
    {
        DeleteAllIndicators();
        if (data == null || data.original_skill == null || data.targets == null || prefab == null) return;

        foreach (BattleChar target in data.targets)
        {
            if (target == null) continue;

            bool affectsEnemy = data.original_skill.affect_enemy;
            Sprite indicatorSprite = affectsEnemy ? ind_atk : ind_ally;
            Sprite smokeSprite = affectsEnemy ? smk_atk : smk_ally;

            if (turn_indicator != null && turn_indicator.character == target)
            {
                turn_indicator.SetSprites(indicatorSprite, smokeSprite);
                turn_indicator.Select(false);
                continue;
            }

            GameObject instance = Instantiate(prefab, transform);
            Indicator indicator = instance.GetComponent<Indicator>();
            if (indicator == null)
            {
                Destroy(instance);
                continue;
            }

            indicator.SetChar(target);
            indicator.SetSprites(indicatorSprite, smokeSprite);
            indicator.Select(false);
            indicators.Add(indicator);
        }
    }

    public void SelectIndicators(BattleChar[] selectedCharacters)
    {
        HashSet<BattleChar> selected = new HashSet<BattleChar>();
        if (selectedCharacters != null)
        {
            foreach (BattleChar target in selectedCharacters)
                if (target != null) selected.Add(target);
        }

        if (turn_indicator != null)
            turn_indicator.Select(selected.Contains(turn_indicator.character));

        foreach (Indicator indicator in indicators)
            if (indicator != null)
                indicator.Select(selected.Contains(indicator.character));
    }

    public void SetMoveIndicators(BattleContext context)
    {
        SetTurnIndicatorDefault();
        if (context == null || context.Self == null || context.Party == null || prefab == null) return;

        int moveRange = context.Self.GetStat(StatType.MOVE);
        for (int offset = -moveRange; offset <= moveRange; offset++)
        {
            int position = offset + context.SelfPosition;
            if (position < 0 || position >= context.Party.Count) continue;

            BattleChar target = context.Party[position];
            if (target == null || target == context.Self || target is BattleObjects) continue;

            GameObject instance = Instantiate(prefab, transform);
            Indicator indicator = instance.GetComponent<Indicator>();
            if (indicator == null)
            {
                Destroy(instance);
                continue;
            }

            indicator.SetChar(target);
            indicator.SetSprites(ind_move, smk_move);
            indicators.Add(indicator);
        }
    }

    public void SelectMoveIndicators(BattleChar character)
    {
        foreach (Indicator indicator in indicators)
            if (indicator != null)
                indicator.Select(indicator.character == character);
    }

    public void DeleteAllIndicators()
    {
        foreach (Indicator indicator in indicators)
            if (indicator != null) Destroy(indicator.gameObject);

        indicators.Clear();
    }

    public void DeleteTurnIndicator()
    {
        if (turn_indicator == null) return;
        Destroy(turn_indicator.gameObject);
        turn_indicator = null;
    }

    public void SubscribeEvents(BattleChar character)
    {
        if (character == null || !subscribedCharacters.Add(character)) return;

        character.OnStartMyTurn += SetTurnIndicator;
        character.OnSkillConfirm += SetTargetIndicators;
        character.OnSkillCancel += SetTurnIndicatorDefault;
        character.OnSkillCancel += DeleteAllIndicators;
        character.OnMoveCancel += DeleteAllIndicators;
        character.OnBattleCharSelect += SelectIndicators;
        character.OnMoveConfirm += SetMoveIndicators;
        character.OnSwapTargetSelect += SelectMoveIndicators;
        character.OnEndMyTurn += DeleteAllIndicators;
        character.OnEndMyTurn += DeleteTurnIndicator;
        character.OnDeath += UnsubscribeEvents;
    }

    public void UnsubscribeEvents(BattleChar character)
    {
        if (character == null || !subscribedCharacters.Remove(character)) return;

        character.OnStartMyTurn -= SetTurnIndicator;
        character.OnSkillConfirm -= SetTargetIndicators;
        character.OnSkillCancel -= SetTurnIndicatorDefault;
        character.OnSkillCancel -= DeleteAllIndicators;
        character.OnMoveCancel -= DeleteAllIndicators;
        character.OnBattleCharSelect -= SelectIndicators;
        character.OnMoveConfirm -= SetMoveIndicators;
        character.OnSwapTargetSelect -= SelectMoveIndicators;
        character.OnEndMyTurn -= DeleteAllIndicators;
        character.OnEndMyTurn -= DeleteTurnIndicator;
        character.OnDeath -= UnsubscribeEvents;
    }

    private void OnDisable()
    {
        foreach (BattleChar character in new List<BattleChar>(subscribedCharacters))
            UnsubscribeEvents(character);

        DeleteAllIndicators();
        DeleteTurnIndicator();
    }

    private void OnDestroy()
    {
        DeleteAllIndicators();
        DeleteTurnIndicator();
    }
}
