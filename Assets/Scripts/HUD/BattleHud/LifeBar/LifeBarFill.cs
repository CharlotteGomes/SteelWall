using Unity.Profiling.Editor;
using UnityEngine;
using UnityEngine.UI;
public class LifeBarFill : MonoBehaviour
{
    Image image;
    void Awake()
    {
        image = GetComponent<Image>();
    }

    
    void Update()
    {
        
    }

    public void UpdateBar(BattleChar character)
    {
        image.fillAmount = (float)character.GetStat(StatType.LIFE_CURRENT) / (float)character.GetStat(StatType.LIFE_MAX);
    }
    private void OnEnable()
    {
        image = GetComponent<Image>();
    }
}
