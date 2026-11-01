using UnityEngine;

public class BattleHud : MonoBehaviour
{
    public PJBattleHUD pj_hud;
    public EnemyBattleHUD enemy_hud;
    public LifeBarManager lifeManager;
    public IndicatorManager indicatorManager;

    public void SetCharacters(BattleChar player, BattleChar enemy)
    {
        SetCharacter(player);
        SetCharacter(enemy);
    }

    public void SetCharacter(BattleChar character)
    {
        if (character is PjController && pj_hud != null)
        {
            pj_hud.SetCharacter(character);
            return;
        }

        if (character is EnemyController && enemy_hud != null)
            enemy_hud.SetCharacter(character);
    }

    public void RefreshHUD()
    {
        if (pj_hud != null) pj_hud.UpdateChildren();
        if (enemy_hud != null) enemy_hud.UpdateChildren();
        if (lifeManager != null) lifeManager.UpdateLifeBars();
    }

    public void OnStartTurn(BattleChar character)
    {
        if (character == null) return;
        SetCharacter(character);
        SubscribeEvents(character);
    }

    public void SubscribeEvents(BattleChar character)
    {
        if (character == null) return;

        if (character is PjController && pj_hud != null)
            pj_hud.SubscriveEvents(character);
        else if (character is EnemyController && enemy_hud != null)
            enemy_hud.SubscriveEvents(character);

        if (indicatorManager != null)
            indicatorManager.SubscribeEvents(character);
    }

    public void UnsubscribeEvents(BattleChar character)
    {
        if (character == null) return;

        if (character is PjController && pj_hud != null)
            pj_hud.UnsubscriveEvents(character);
        else if (character is EnemyController && enemy_hud != null)
            enemy_hud.UnsubscriveEvents(character);

        if (indicatorManager != null)
            indicatorManager.UnsubscribeEvents(character);
    }
}
