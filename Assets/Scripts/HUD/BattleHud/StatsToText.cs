using TMPro;
using UnityEngine;

public class StatsToText : MonoBehaviour
{
    [SerializeField] private StatType statType1;
    [SerializeField] private StatType statType2;

    private BattleChar character;
    private TMP_Text text;

    private void Awake()
    {
        CacheReferences();
    }

    private void OnEnable()
    {
        CacheReferences();
    }

    private void CacheReferences()
    {
        if (text == null)
            text = GetComponent<TMP_Text>();
    }

    public void UpdateCharacter(BattleChar target)
    {
        character = target;
    }

    public void UpdateText()
    {
        if (character == null || text == null) return;

        int firstValue = character.GetStat(statType1);
        text.text = statType2 != StatType.NOONE
            ? $"{firstValue}-{character.GetStat(statType2)}"
            : firstValue.ToString();
    }
}
