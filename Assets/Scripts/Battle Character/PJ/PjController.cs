using System.Collections;
using UnityEngine;

public class PjController : BattleChar
{
    private void Awake()
    {
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J)) { StartCoroutine(StartSkill()); }
    }

    public override IEnumerator StartSkill()
    {
        base.StartSkill();
        Debug.Log($"Inicializando {skill_current.Name}");
        yield return StartCoroutine(OnGoingSkill());
        ExecuteSkillsEffects();
    }

    public override IEnumerator OnGoingSkill()
    {
        float time = Time.time;
        float qte_start = skill_current.qte_start + time;
        float qte_end = skill_current.qte_end + time;
        bool crit = false;
        while (Time.time < qte_end && crit == false)
        {
            InputManager.GetInputs();
            if (Time.time > qte_start)
            {
                if (InputManager.finisher) { ExecuteFinisher(); Debug.Log("Critou"); crit = true; }
            }
            yield return null;
         }
    }

    void ExecuteFinisher()
    {
        if(skill_current == null) { return;}
        foreach (SkillEffectSO f in skill_current.finisher)
        {
            f.Execute(ctx);
        }
    }
}
