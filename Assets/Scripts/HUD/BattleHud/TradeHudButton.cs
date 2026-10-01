using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TradeHudButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Image image;
    [SerializeField] PJBattleHUD panel;
    [SerializeField] Sprite sprite_pressed, sprite_normal;
    [SerializeField] GameObject atributos, resistencias;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        TradeStatus();
    }

    void TradeStatus()
    {
        if (atributos.activeSelf)
        {
            atributos.SetActive(false);
            resistencias.SetActive(true);
            image.sprite = sprite_pressed;
            panel.UpdateChildren();
            return;
        }

        atributos.SetActive(true);
        resistencias.SetActive(false);
        image.sprite = sprite_normal;
        panel.UpdateChildren();
    }
}
