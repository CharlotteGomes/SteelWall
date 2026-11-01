using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using TMPro;
public class PJBattleHUD : StatsPanel
{
    [SerializeField] List<SkillIcon> skillIcon = new List<SkillIcon>();
    [SerializeField] SwapButton swapButton;
    [SerializeField] SkipButton skipButton;
    [SerializeField] ButtonSelect buttonSelect;
    [Header("Optional skill details")]
    [SerializeField] TMP_Text skillDescriptionText;

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
        int visibleSkillCount = Mathf.Min(pj.read_skills.Count, skillIcon.Count);
        for (int i = 0; i < visibleSkillCount; i++)
        {
            if (skillIcon[i] == null || pj.read_skills[i] == null) continue;
            skillIcon[i].UpdateSkillSO(pj.read_skills[i]);
            skillIcon[i].UpdateSprite();
        }
        for (int i = visibleSkillCount; i < skillIcon.Count; i++)
            if (skillIcon[i] != null) skillIcon[i].UpdateSkillSO(null);
    }

    public void SetSkill(SkillSO so)
    {
        if (character is not PjController pj) return;
        pj.OnSkillCancel?.Invoke();
        pj.SetSkill(so);
        pj.ConfirmSkill();
        SelectSkill(so);
        RefreshSkillDescription();
    }

    public void SelectSkill(SkillSO so)
    {
        if (skillDescriptionText != null)
            skillDescriptionText.text = SkillDescriptionBuilder.Build(so, character);
        if (character is not PjController pj) return;
        int count = Mathf.Min(pj.read_skills.Count, skillIcon.Count);
        for (int i = 0; i < count; i++)
        {
            if (pj.read_skills[i] == so && skillIcon[i] != null)
                SetButtonSelect<SkillIcon>(skillIcon[i]);
        }
    }

    private void HandleSkillConfirmed(SkillData skillData)
    {
        if (skillDescriptionText != null)
            skillDescriptionText.text = SkillDescriptionBuilder.Build(skillData, character);
    }

    private void HandleTargetsChanged(BattleChar[] targets)
    {
        if (character is PjController pj && pj.CurrentSkillData != null && skillDescriptionText != null)
            skillDescriptionText.text = SkillDescriptionBuilder.Build(pj.CurrentSkillData, character);
    }

    private void RefreshSkillDescription()
    {
        if (character is PjController pj && pj.CurrentSkillData != null)
            HandleSkillConfirmed(pj.CurrentSkillData);
        else if (character is PjController player && player.skill_read != null)
            SelectSkill(player.skill_read);
    }

    private void ClearSkillDescription()
    {
        if (skillDescriptionText != null) skillDescriptionText.text = string.Empty;
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
        character.OnSkillConfirm += HandleSkillConfirmed;
        character.OnBattleCharSelect += HandleTargetsChanged;
        character.OnSkillCancel += ClearSkillDescription;
    }

    public override void UnsubscriveEvents(BattleChar character)
    {
        base.UnsubscriveEvents(character);
        character.OnSkillSelect -= SelectSkill;
        character.OnMoveActionSelect -= SelectMoveButton;
        character.OnSkipSelect -= SelectSkipButton;
        character.OnSkillConfirm -= HandleSkillConfirmed;
        character.OnBattleCharSelect -= HandleTargetsChanged;
        character.OnSkillCancel -= ClearSkillDescription;
    }
}
