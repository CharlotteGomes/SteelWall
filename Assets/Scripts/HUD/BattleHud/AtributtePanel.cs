using UnityEngine;
using System.Collections.Generic;
public class AtributtePanel : MonoBehaviour
{
    [SerializeField] BattleChar character;
    [SerializeField] List<StatsToText> statsToText = new List<StatsToText>();
    [SerializeField] List<SkillIcon> skillIcon = new List<SkillIcon>();
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) { UpdateChildren(); }
    }

    public void SetCharacter(BattleChar character)
    {
        this.character = character;
    }

    public void UpdateChildren()
    {
        foreach(StatsToText t in statsToText)
        {
            if(!t.isActiveAndEnabled) continue;
            t.UpdateCharacter(character);
            t.UpdateText();
        }

        if (character is PjController pj)
        {
            for (int i = 0; i < pj.read_skills.Count; i++)
            {
                skillIcon[i].UpdateSkillSO(pj.read_skills[i]);
                skillIcon[i].UpdateSprite();
            }
            SelectSkillIcon(0);
        }

        
    }

    public void SetSkill(SkillSO so)
    {
        if (character is not PjController pj) return;
        for (int i = 0; i < skillIcon.Count; i++) 
        {
            SkillIcon ss = skillIcon[i];
            if(ss.so == so)
            {
                pj.SetSkill(so);
                pj.SetActionIndex(i);
                SelectSkillIcon(i);
                continue;
            }
        }
    }

    public void SelectSkillIcon(int index)
    {
        for (int i = 0; i < skillIcon.Count; i++)
        {
            SkillIcon ss = skillIcon[i];
            if (i == index)
            {
                ss.select = true;
                ss.onSelect.Select();
                continue;
            }   
            ss.select = false;
            ss.onSelect.Diselect();
        }
    }

    public void DiselectAllSkillIcon()
    {
        foreach(SkillIcon ss in skillIcon)
        {
            ss.select = false;
            ss.onSelect.Diselect();
        }
    }
}
