using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
public class LifeBar : MonoBehaviour
{
    public BattleCharacter character;
    [SerializeField] RectTransform LifeCurrent;
    [SerializeField] TMP_Text LifeNumber;
    RectTransform canvasRect, rectTransform;
    public void SettingCharacter(BattleCharacter character)
    {
        this.character = character;
        Vector3 worldPos = character.transform.position + Vector3.down * 0.7f;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPos,
            Camera.main,
            out Vector2 uiPos
        );
        rectTransform.anchoredPosition = uiPos;
    }
    
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasRect = GameObject.FindGameObjectWithTag("Canvas").GetComponent<RectTransform>();
    }

    void Update()
    {
        if(character == null) { return; }
        LifeNumber.text = $"{character.lifeCurrent}/{character.LifeMax}";
        LifeCurrent.localScale = new Vector2((float)character.lifeCurrent / character.LifeMax, 1); 
    }
}
