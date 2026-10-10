using TMPro;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;
public class StatsToText : MonoBehaviour
{
    [SerializeField] StatType statType1, statType2;
    BattleChar character = null; 
    TMP_Text tmp;
    void Start()
    {
        tmp = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateCharacter(BattleChar c)
    {
        character = c;
    }

    public void UpdateText()
    {
        if (character == null) return;
        if (statType2 != StatType.NOONE)
        {
            tmp.text = $"{character.GetStat(statType1)}-{character.GetStat(statType2)}";
            return;
        }

        tmp.text = $"{character.GetStat(statType1)}";
        return;
    }
    private void OnEnable()
    {
        tmp = GetComponent<TMP_Text>();
    }
}
