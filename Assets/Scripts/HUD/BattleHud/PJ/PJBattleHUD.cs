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
        pj.OnSkillCancel?.Invoke();
        pj.SetSkill(so);
        pj.ConfirmSkill();
        SelectSkill(so);
    }

    public void SelectSkill(SkillSO so)
    {
        if (character is not PjController pj) return;
        for(int i = 0; i < pj.read_skills.Count; i++)
        {
            if (pj.read_skills[i] == so)
            {
                SetButtonSelect<SkillIcon>(skillIcon[i]);
                continue;
            }
        }
    }

    public void SetMoveAction()
    {
        if (character is not PjController pj) return;
        pj.OnMoveCancel?.Invoke();
        SelectMoveButton();
        pj.ConfirmMoveAction();
    }

    public void SelectMoveButton()
    {
        SetButtonSelect<SwapButton>(swapButton);
    }

    public void SelectSkipButton()
    {
        SetButtonSelect<SkipButton>(skipButton);
    }

    public void DisableButtonSelect()
    {
        buttonSelect.gameObject.SetActive(false);
    }

    public void SetButtonSelect<T>(T target) where T : MonoBehaviour
    {
        if(!buttonSelect.gameObject.activeSelf) buttonSelect.gameObject.SetActive(true);
        RectTransform rect = target.GetComponent<RectTransform>();
        StartCoroutine(buttonSelect.LerpTransform(rect));
    }

    public override void SubscriveEvents(BattleChar character)
    {
        base.SubscriveEvents(character);
        character.OnSkillSelect += SelectSkill;
        character.OnMoveActionSelect += SelectMoveButton;
        character.OnSkipSelect += SelectSkipButton;
    }

    public override void UnsubscriveEvents(BattleChar character)
    {
        base.UnsubscriveEvents(character);
        character.OnSkillSelect -= SelectSkill;
        character.OnMoveActionSelect -= SelectMoveButton;
        character.OnSkipSelect -= SelectMoveButton;
    }
}
