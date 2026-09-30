using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class OnSelect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Coroutine coroutine;
    [SerializeField] float up_scale;
    [SerializeField] bool onClick;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (onClick) return; 
        Select();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (onClick) return;
        Diselect();
    }

    IEnumerator UpScale()
    {
        float t = 0f;
        float duration = 0.2f;
        Vector2 scale = transform.localScale;
        while(t < duration)
        {
            t += Time.deltaTime / duration;
            float curve = 1f - Mathf.Pow(1f - t, 3f);
            transform.localScale = Vector2.Lerp(scale, Vector2.one * up_scale, curve);
            yield return null;
        }
        transform.localScale = Vector2.one * up_scale;
        yield return null;
    }

    IEnumerator DownScale()
    {
        float t = 0f;
        float duration = 0.2f;
        Vector2 scale = transform.localScale;
        while (t < duration)
        {
            t += Time.deltaTime / duration;
            float curve = 1f - Mathf.Pow(1f - t, 3f);
            transform.localScale = Vector2.Lerp(scale, Vector2.one, curve);
            yield return null;
        }
        transform.localScale = Vector2.one;
        yield return null;
    }

    public void Select()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
        coroutine = StartCoroutine(UpScale());
    }

    public void Diselect()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
        coroutine = StartCoroutine(DownScale());
    }
}
