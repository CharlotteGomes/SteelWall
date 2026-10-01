using UnityEngine;
using UnityEngine.EventSystems;

public class SkipButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] PJBattleHUD panel;

    public void OnPointerClick(PointerEventData eventData)
    {
        panel.SetSkipTurn();
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
