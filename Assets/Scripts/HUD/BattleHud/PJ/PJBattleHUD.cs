using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PJBattleHUD : StatsPanel
{
    [SerializeField] List<SkillIcon> skillIcon = new List<SkillIcon>();
    [SerializeField] SwapButton swapButton;
    [SerializeField] SkipButton skipButton;
    [SerializeField] ButtonSelect buttonSelect;
    [Header("Optional skill details")]
    [SerializeField] TMP_Text skillDescriptionText;

    public override void UpdateChildren()
    {
        base.UpdateChildren();
        if (character is not PjController player) return;

        int visibleSkillCount = Mathf.Min(player.read_skills.Count, skillIcon.Count);
        for (int i = 0; i < skillIcon.Count; i++)
        {
            SkillIcon icon = skillIcon[i];
            if (icon == null) continue;

            SkillSO skill = i < visibleSkillCount ? player.read_skills[i] : null;
            icon.UpdateSkillSO(skill);
            icon.UpdateSprite();
        }
    }

    public void SetSkill(SkillSO skill)
    {
        if (skill == null || character is not PjController player) return;

        player.OnSkillCancel?.Invoke();
        player.SetSkill(skill);
        player.ConfirmSkill();
        SelectSkill(skill);
        RefreshSkillDescription();
    }

    public void SelectSkill(SkillSO skill)
    {
        if (skillDescriptionText != null)
            skillDescriptionText.text = SkillDescriptionBuilder.Build(skill, character);

        if (character is not PjController player || skill == null) return;
        int count = Mathf.Min(player.read_skills.Count, skillIcon.Count);
        for (int i = 0; i < count; i++)
        {
            if (player.read_skills[i] == skill && skillIcon[i] != null)
            {
                SetButtonSelect(skillIcon[i]);
                break;
            }
        }
    }

    private void HandleSkillConfirmed(SkillData skillData)
    {
        if (skillDescriptionText != null)
            skillDescriptionText.text = SkillDescriptionBuilder.Build(skillData, character);
    }

    private void HandleTargetsChanged(BattleChar[] targets)
    {
        if (character is PjController player && player.CurrentSkillData != null && skillDescriptionText != null)
            skillDescriptionText.text = SkillDescriptionBuilder.Build(player.CurrentSkillData, character);
    }

    private void RefreshSkillDescription()
    {
        if (character is PjController player && player.CurrentSkillData != null)
            HandleSkillConfirmed(player.CurrentSkillData);
        else if (character is PjController currentPlayer && currentPlayer.skill_read != null)
            SelectSkill(currentPlayer.skill_read);
    }

    private void ClearSkillDescription()
    {
        if (skillDescriptionText != null)
            skillDescriptionText.text = string.Empty;
    }

    public void SetMoveAction()
    {
        if (character is not PjController player) return;
        player.OnMoveCancel?.Invoke();
        SelectMoveButton();
        player.ConfirmMoveAction();
    }

    public void SelectMoveButton()
    {
        if (swapButton != null) SetButtonSelect(swapButton);
    }

    public void SelectSkipButton()
    {
        if (skipButton != null) SetButtonSelect(skipButton);
    }

    public void DisableButtonSelect()
    {
        if (buttonSelect != null)
            buttonSelect.gameObject.SetActive(false);
    }

    public void SetButtonSelect<T>(T target) where T : MonoBehaviour
    {
        if (buttonSelect == null || target == null) return;

        if (!buttonSelect.gameObject.activeSelf)
            buttonSelect.gameObject.SetActive(true);

        RectTransform rect = target.GetComponent<RectTransform>();
        if (rect != null)
            buttonSelect.MoveTo(rect);
    }

    public override void SubscriveEvents(BattleChar target)
    {
        if (!TryBeginSubscription(target)) return;

        target.OnSkillSelect += SelectSkill;
        target.OnMoveActionSelect += SelectMoveButton;
        target.OnSkipSelect += SelectSkipButton;
        target.OnSkillConfirm += HandleSkillConfirmed;
        target.OnBattleCharSelect += HandleTargetsChanged;
        target.OnSkillCancel += ClearSkillDescription;
    }

    public override void UnsubscriveEvents(BattleChar target)
    {
        if (!TryEndSubscription(target)) return;

        target.OnSkillSelect -= SelectSkill;
        target.OnMoveActionSelect -= SelectMoveButton;
        target.OnSkipSelect -= SelectSkipButton;
        target.OnSkillConfirm -= HandleSkillConfirmed;
        target.OnBattleCharSelect -= HandleTargetsChanged;
        target.OnSkillCancel -= ClearSkillDescription;
    }
}
