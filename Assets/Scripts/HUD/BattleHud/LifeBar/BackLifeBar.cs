using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BackLifeBar : MonoBehaviour
{
    Image image;

    private void Awake() { image = GetComponent<Image>(); }
    private void OnEnable() { if (image == null) image = GetComponent<Image>(); }

    public IEnumerator UpdateBar(BattleChar character)
    {
        if (image == null || character == null) yield break;
        yield return new WaitForSeconds(0.5f);
        if (character == null) yield break;

        float maxLife = character.GetStat(StatType.LIFE_MAX);
        float target = maxLife > 0
            ? Mathf.Clamp01((float)character.GetStat(StatType.LIFE_CURRENT) / maxLife)
            : 0f;
        float start = image.fillAmount;
        const float duration = 2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (character == null) yield break;
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float curve = 1f - Mathf.Pow(1f - t, 3f);
            image.fillAmount = Mathf.Lerp(start, target, curve);
            yield return null;
        }
        image.fillAmount = target;
    }
}