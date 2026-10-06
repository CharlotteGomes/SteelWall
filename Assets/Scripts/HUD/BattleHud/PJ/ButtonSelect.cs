using NUnit.Framework.Constraints;
using System.Collections;
using UnityEngine;

public class ButtonSelect : MonoBehaviour
{
    RectTransform tf;
    void Start()
    {
        tf = GetComponent<RectTransform>();
    }

    void Update()
    {
        
    }

    private void OnEnable()
    {
        tf = GetComponent<RectTransform>();
    }
  
    public IEnumerator LerpTransform(RectTransform _tf)
    {
        Vector2 StartPos = tf.position;
        Vector2 EndPos = _tf.position;
        Vector2 StartScale = tf.sizeDelta;
        Vector2 EndScale = _tf.sizeDelta + Vector2.one * 35;
        float t = 0f;
        float duration = 0.35f;

        while (t < duration)
        {
            t += Time.deltaTime / duration; 
            float curve = 1f - Mathf.Pow(1f - t, 3f);
            tf.position = Vector2.Lerp(StartPos, EndPos, curve);
            tf.sizeDelta = Vector2.Lerp(StartScale, EndScale, curve);
            yield return null;
        }
        tf.position = EndPos;
        tf.sizeDelta = EndScale;
        yield return null;
    }
}
