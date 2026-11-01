using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TradeHudButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image image;
    [SerializeField] private PJBattleHUD panel;
    [SerializeField] private Sprite sprite_pressed, sprite_normal;
    [SerializeField] private GameObject atributos, resistencias;

    public void OnPointerClick(PointerEventData eventData)
    {
        TradeStatus();
    }

    private void TradeStatus()
    {
        if (atributos == null || resistencias == null) return;

        bool showAttributes = !atributos.activeSelf;
        atributos.SetActive(showAttributes);
        resistencias.SetActive(!showAttributes);

        if (image != null)
            image.sprite = showAttributes ? sprite_normal : sprite_pressed;

        if (panel != null)
            panel.UpdateChildren();
    }
}
