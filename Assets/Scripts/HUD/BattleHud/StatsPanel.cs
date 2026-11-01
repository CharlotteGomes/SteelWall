using System.Collections.Generic;
using UnityEngine;

public class StatsPanel : MonoBehaviour
{
    [SerializeField] protected BattleChar character;
    [SerializeField] protected List<StatsToText> statsToText = new List<StatsToText>();

    private BattleChar subscribedCharacter;

    public void SetCharacter(BattleChar newCharacter)
    {
        if (subscribedCharacter != null && subscribedCharacter != newCharacter)
            UnsubscriveEvents(subscribedCharacter);

        character = newCharacter;
        UpdateChildren();
    }

    public virtual void UpdateChildren()
    {
        foreach (StatsToText textField in statsToText)
        {
            if (textField == null || !textField.isActiveAndEnabled) continue;
            textField.UpdateCharacter(character);
            textField.UpdateText();
        }
    }

    // Kept with the existing method names to preserve derived classes and serialized scene references.
    public virtual void SubscriveEvents(BattleChar target)
    {
        TryBeginSubscription(target);
    }

    public virtual void UnsubscriveEvents(BattleChar target)
    {
        TryEndSubscription(target);
    }

    protected bool TryBeginSubscription(BattleChar target)
    {
        if (target == null || subscribedCharacter == target) return false;

        if (subscribedCharacter != null)
            UnsubscriveEvents(subscribedCharacter);

        subscribedCharacter = target;
        return true;
    }

    protected bool TryEndSubscription(BattleChar target)
    {
        if (target == null || subscribedCharacter != target) return false;
        subscribedCharacter = null;
        return true;
    }

    protected void OnEnable()
    {
        if (character != null)
            SubscriveEvents(character);
    }

    protected void OnDisable()
    {
        if (subscribedCharacter != null)
            UnsubscriveEvents(subscribedCharacter);
    }
}
