using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class BackLifeBar : MonoBehaviour
{
    Image image;
    void Awake()
    {
        image= GetComponent<Image>();
    }

    
    void Update()
    {

    }

    public IEnumerator UpdateBar(BattleChar character)
    {
        yield return new WaitForSeconds(0.5f);
        float t = 0;
        float duration = 2f;

        float x = image.fillAmount;
        float target = (float)character.GetStat(StatType.LIFE_CURRENT) / (float)character.GetStat(StatType.LIFE_MAX);
        while (t < duration - .1f)
        {
            t += Time.deltaTime / duration;
            float curve = 1f - Mathf.Pow(1f - t, 3f);
            image.fillAmount = Mathf.Lerp(x, target, curve);
            yield return null;
        }
        image.fillAmount = target;
    }

    private void OnEnable()
    {
        image = GetComponent<Image>();
    }
}
