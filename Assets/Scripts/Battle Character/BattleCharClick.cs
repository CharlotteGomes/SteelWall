using UnityEngine;
using UnityEngine.EventSystems;

public class BattleCharClick : MonoBehaviour, IPointerClickHandler
{
    PjController pj;
    public void OnPointerClick(PointerEventData data)
    {
        BattleHUD hud = GM.instance.battle_manager.hud;
        hud.SetCurrent(pj);
        hud.UpdateSkills();
        Debug.Log("To clicando");
    }

    void Start()
    {
        pj = GetComponent<PjController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
