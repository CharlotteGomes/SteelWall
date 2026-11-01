using UnityEngine;

public class EnemyBattleHUD : StatsPanel
{
    [SerializeField] private ShowText text;

    public override void UpdateChildren()
    {
        base.UpdateChildren();

        if (text == null || character == null) return;
        text.UpdateText(character.name);
        text.UpdateTMP();
    }
}
