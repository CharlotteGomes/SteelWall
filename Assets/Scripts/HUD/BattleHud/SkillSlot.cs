using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class SkillSlot : MonoBehaviour, IPointerClickHandler
{
    public SkillSO skill;
    Image image;
    public void OnPointerClick(PointerEventData eventData)
    {
        BattleHUD hud = GM.instance.battle_manager.hud;
        BattleManager manager = GM.instance.battle_manager;

        if(manager.current == hud.current)
        {
            manager.current.SetSkill(skill);
            hud.SetSelectedSlot(this);
        }
    }

    void Start()
    {
        image = GetComponent<Image>();  
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) UpdateImage();
    }

    public void UpdateImage()
    {
        image.sprite = skill.spr;
    }
}
