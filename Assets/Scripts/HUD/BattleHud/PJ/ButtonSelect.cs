using NUnit.Framework.Constraints;
using System.Collections;
using UnityEngine;

public class ButtonSelect : MonoBehaviour
{
    Transform tf;
    void Start()
    {
        tf = transform;
    }

    void Update()
    {
        
    }

    private void OnEnable()
    {
        tf = transform;
    }

    public void SelectParent(Transform parent)
    {
        tf.SetParent(parent);
    }
   
    public void GoToParent()
    {
        StartCoroutine(LerpPosition());
    }
    IEnumerator LerpPosition()
    {
        Vector2 StartPos = tf.localPosition;
        Vector2 EndPos = Vector2.zero;
        float t = 0f;
        float duration = 0.35f;

        while (t < duration)
        {
            t += Time.deltaTime / duration; 
            float curve = 1f - Mathf.Pow(1f - t, 3f);
            tf.localPosition = Vector2.Lerp(StartPos, EndPos, curve);
            yield return null;
        }
        tf.localPosition = EndPos;
        yield return null;
    }
}
