using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TwentyFiveSlicer.Runtime;
public class Indicator : MonoBehaviour
{
    public TwentyFiveSliceImage ind_image, smk_image;
    public bool animate = true;
    [SerializeField] CanvasGroup group;
    public BattleChar character { get; private set; }   
    void Awake()
    {
        group = GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        group = GetComponent<CanvasGroup>();
    }

    public void SetChar(BattleChar character)
    {
        this.character = character;
        SetPosition();
    }

    public void SetPosition()
    {
        transform.position = (Vector2)character.transform.position + Vector2.down * .05f;
        transform.localScale = Vector2.one;
    }

    public void SetSprites(Sprite ind,  Sprite smk)
    {
        ind_image.enabled = ind != null ? true : false;
        smk_image.enabled = smk != null ? true : false;
        ind_image.sprite = ind !=null? ind : null ;
        smk_image.sprite = smk != null? smk : null;
    }

    public void Select(bool select)
    {
        if (select) {group.alpha = 1; return; }
        group.alpha = .30f;
    }

    public IEnumerator StartIndAnimation(BattleChar c)
    {
        float size = transform.localScale.x;
        float originalScale =  size;
        float targetScale = 1.25f * size;
        float t = 0f;
        float dur = 0.75f * size;
        while (animate)
        {
            t += Time.deltaTime / dur;
            float curve = 1 - Mathf.Pow(1 - t, 3);
            transform.localScale = Vector2.Lerp(new Vector2(originalScale, 1), new Vector2(targetScale, 1), curve);
            if(curve >= 0.95f)
            {
                t = 0;
                dur = dur == 0.75f * size ? 0.33f * size : 0.75f * size;
                float a = originalScale;
                float b = targetScale;
                originalScale = b;
                targetScale = a;
            }
            yield return null;
        }
        yield return null;
    }

    private void OnDestroy()
    {
        animate = false;
    }
}
