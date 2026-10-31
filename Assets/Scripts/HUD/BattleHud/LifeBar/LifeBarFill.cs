using UnityEngine;
using UnityEngine.UI;

public class LifeBarFill : MonoBehaviour
{
    Image image;
    private void Awake() { image = GetComponent<Image>(); }
    private void OnEnable() { if (image == null) image = GetComponent<Image>(); }

    public void UpdateBar(BattleChar character)
    {
        if (image == null || character == null) return;
        float maxLife = character.GetStat(StatType.LIFE_MAX);
        image.fillAmount = maxLife > 0
            ? Mathf.Clamp01((float)character.GetStat(StatType.LIFE_CURRENT) / maxLife)
            : 0f;
    }
}