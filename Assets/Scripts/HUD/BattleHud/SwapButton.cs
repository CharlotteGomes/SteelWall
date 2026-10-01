using UnityEngine;
using UnityEngine.EventSystems;

public class SwapButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] PJBattleHUD panel;

    public void OnPointerClick(PointerEventData eventData)
    {
        panel.SetMoveAction();
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable()
    {
    }
}
