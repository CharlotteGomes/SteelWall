using System.Collections.Generic;
using UnityEngine;

public class LifeBarManager : MonoBehaviour
{
    [SerializeField] private GameObject life_bar;
    [SerializeField] private List<LifeBar> lifeBars = new List<LifeBar>();

    public void UpdateLifeBars()
    {
        for (int i = lifeBars.Count - 1; i >= 0; i--)
        {
            LifeBar lifeBar = lifeBars[i];
            if (lifeBar == null)
            {
                lifeBars.RemoveAt(i);
                continue;
            }

            lifeBar.UpdateBars();
        }
    }

    public void SpawnBars(List<BattleChar> characters)
    {
        DeleteBars();
        if (characters == null) return;

        foreach (BattleChar character in characters)
            IndividualSpawn(character);
    }

    public void IndividualDelete(BattleChar character)
    {
        for (int i = lifeBars.Count - 1; i >= 0; i--)
        {
            LifeBar bar = lifeBars[i];
            if (bar == null)
            {
                lifeBars.RemoveAt(i);
                continue;
            }

            if (bar.character != character) continue;
            lifeBars.RemoveAt(i);
            Destroy(bar.gameObject);
        }
    }

    public void IndividualSpawn(BattleChar character)
    {
        if (character == null || life_bar == null) return;

        foreach (LifeBar existing in lifeBars)
            if (existing != null && existing.character == character) return;

        GameObject instance = Instantiate(life_bar, transform, false);
        LifeBar life = instance.GetComponent<LifeBar>();
        if (life == null)
        {
            Debug.LogError("The configured life bar prefab is missing its LifeBar component.", instance);
            Destroy(instance);
            return;
        }

        life.SetCharacter(character);
        lifeBars.Add(life);
    }

    public void MoveBars()
    {
        foreach (LifeBar bar in lifeBars)
            if (bar != null) bar.SetPos();
    }

    public void DeleteBars()
    {
        foreach (LifeBar bar in lifeBars)
            if (bar != null) Destroy(bar.gameObject);

        lifeBars.Clear();
    }

    public void HideBars()
    {
        foreach (LifeBar bar in lifeBars)
            if (bar != null) bar.SetVisible(false);
    }

    public void ShowBars()
    {
        foreach (LifeBar bar in lifeBars)
            if (bar != null) bar.SetVisible(true);
    }

    private void OnDestroy()
    {
        DeleteBars();
    }
}
