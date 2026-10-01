using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
public class PJBattleHUD : StatsPanel
{
    [SerializeField] List<SkillIcon> skillIcon = new List<SkillIcon>();
    [SerializeField] SwapButton swapButton;
    [SerializeField] SkipButton skipButton;
    [SerializeField] ButtonSelect buttonSelect;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) { UpdateChildren(); }
    }

    public override void UpdateChildren()
    {
        base.UpdateChildren();
        if (character is not PjController pj) return;
        for (int i = 0; i < pj.read_skills.Count; i++)
        {
            skillIcon[i].UpdateSkillSO(pj.read_skills[i]);
            skillIcon[i].UpdateSprite();
        }
    }

    public void SetSkill(SkillSO so)
    {
        if (character is not PjController pj) return;
        pj.SetSkill(so);
        pj.SelectChooseSkill();
        pj.ConfirmSkill();
        SelectSkill(pj.FindSkillIndex(so));
    }

    public void SelectSkill(int index = -1)
    {
        if (character is not PjController pj) return;
        for(int i = 0; i < pj.read_skills.Count; i++)
        {
            if (i == index)
            {
                SetButtonSelect<SkillIcon>(skillIcon[i]);
                continue;
            }
        }
    }

    public void SetMoveAction()
    {
        if (character is not PjController pj) return;
        pj.SelectMoveAction();
        pj.ConfirmMoveAction();
    }

    public void SelectMoveButton()
    {
        SetButtonSelect<SwapButton>(swapButton);
    }

    public void SetSkipTurn()
    {
        if (character is not PjController pj) return;
        pj.SelectSkipTurn();

    }

    public void SelectSkipButton(bool select = true)
    {

    }

    public void DisableButtonSelect()
    {
        buttonSelect.gameObject.SetActive(false);
    }

    public void SetButtonSelect<T>(T target) where T : MonoBehaviour
    {
        if(!buttonSelect.gameObject.activeSelf) buttonSelect.gameObject.SetActive(true);
        buttonSelect.SelectParent(target.transform);
        buttonSelect.GoToParent();
    }
}
