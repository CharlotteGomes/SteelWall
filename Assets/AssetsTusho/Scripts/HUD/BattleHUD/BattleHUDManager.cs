using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class BattleHUDManager : MonoBehaviour
{
    [SerializeField] GameObject SkillBar, LifeBar;
    [SerializeField] SkillBar skillbar;

    [SerializeField] List<LifeBar> lifebars = new List<LifeBar>();

    public void ShowPjLifeBar(List<BattleCharacter> pjs)
    {
        int i = 0;
        foreach (BattleCharacter pj in pjs)
        {
            GameObject lifebar = Instantiate(LifeBar, transform);
            lifebars.Add(lifebar.GetComponent<LifeBar>());
            lifebars[i].SettingCharacter(pj);
        }
    }

    public void ShowSkillBar(PJController pj)
    {
        GameObject bar = Instantiate(SkillBar, transform);
        skillbar = bar.GetComponent<SkillBar>();
        skillbar.SettingCharacter(pj);
        skillbar.GetSkills();
        skillbar.SpawnSlots();
        skillbar.RefreshSlots();
        skillbar.MoveAndResize(0);
    }

    public void DeleteSkillBar()
    {
        Destroy(skillbar.gameObject);
    }

    public void UpdateSkillBar(int idx)
    {
        skillbar.MoveAndResize(idx);
    }

    public void SubEvent(PJController pj)
    {
        pj.OnSkillChanged += UpdateSkillBar;
        pj.OnPhaseChanged += PJChangePhase;
    }

    public void UnsubEvent(PJController pj)
    {
        pj.OnSkillChanged -= UpdateSkillBar;
        pj.OnPhaseChanged -= PJChangePhase;
    }

    public void PJChangePhase(TurnPhase newphase, PJController pj)
    {
        switch (newphase)
        {
            case TurnPhase.ChoosingSkill:
                ShowSkillBar(pj); break;

            case TurnPhase.ChoosingTarget:
               DeleteSkillBar() ;break;
            case TurnPhase.Finished:
                UnsubEvent(pj);break;   

        }
    }
    void Start()
    {   
        
    }

    // Update is called once per frame
    void Update()
    { 

    }
}
