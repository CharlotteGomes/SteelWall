using System.Collections;
using UnityEngine;

public class PjController : BattleChar
{
    [SerializeField] SkillSO test;
    private void Awake()
    {
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            AddSkill(test);
            EquipSkill(skills_unlocked[0]);
            skill_current = skill_equipped[0];
        }
        if (Input.GetKeyDown(KeyCode.K)){StartCoroutine(StartSkill());
        }
        if (Input.GetKeyDown(KeyCode.DownArrow)) ReduceCooldown();
    }

    public override IEnumerator StartSkill()
    {
        if(skill_current.cooldown != 0) { yield break; }
        yield return base.StartSkill();
    }

    public override IEnumerator OnGoingSkill()
    {
        /*
        float time = Time.time;
        float qte_start = skill_current.qte_start + time;
        float qte_end = skill_current.qte_end + time;
        bool crit = false;
        while (Time.time < skill_current.clip.length + time)
        {
            InputManager.GetInputs();
            if (Time.time > qte_start && crit == false)
            {
                if (InputManager.finisher) { ExecuteFinisher(); Debug.Log("Critou"); crit = true; }
            }
            yield return null;
         }*/
        yield return StartCoroutine(FinishingSkill());
    }

    void ExecuteFinisher()
    {
        /*if(skill_current == null) { return;}
        foreach (SkillEffectSO f in skill_current.finisher)
        {
            f.Execute(ctx);
        }*/
    }
}
