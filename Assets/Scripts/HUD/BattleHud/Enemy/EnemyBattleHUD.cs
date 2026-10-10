using UnityEngine;

public class EnemyBattleHUD : StatsPanel
{
    [SerializeField] ShowText text;

    public override void UpdateChildren()
    {
        base.UpdateChildren();
        text.UpdateText(character.name);
        text.UpdateTMP();
    }
}
