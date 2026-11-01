using UnityEngine;
using UnityEngine.EventSystems;

public class SwapButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private PJBattleHUD panel;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (panel != null)
            panel.SetMoveAction();
    }
}
