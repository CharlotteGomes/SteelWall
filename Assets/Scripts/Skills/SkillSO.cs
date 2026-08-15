using JetBrains.Annotations;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillSO", menuName = "Scriptable Objects/SkillSO/Skill")]
public class SkillSO : ScriptableObject
{
    public string Name, Description;
    public Sprite spr;
    public int cooldown;
    public int cost;
    public int precision;
    public SkillEffectSO[] effects;
    public SkillEffectSO[] finisher;
    public float skill_duration_time = 0;
    public bool criticable = false;
    public float min_finisher_time = 0;
    public float max_finisher_time = 0;
    public void ExecuteSkill(BattleContext ctx)
    {
        GM.instance.RunCoroutine(CritWindow(ctx));
    }

    public void ExecuteEffect(BattleContext ctx)
    {
        foreach (SkillEffectSO f in effects)
        {
            f.Execute(ctx);
        }
    }

    public void ExecuteFinisher(BattleContext ctx)
    {
        foreach (SkillEffectSO f in finisher)
        {
            f.Execute(ctx);
        }
    }

    IEnumerator CritWindow(BattleContext ctx)
    {
        //Starta animação
        //inicia contagem
        bool can_crit = criticable ? true : false;
        bool can_finisher = finisher.Length > 0 ? true : false;
        float start_anim = Time.time;
        float min_crit = min_finisher_time + start_anim;
        float max_crit = max_finisher_time + start_anim;
        float duration = skill_duration_time + start_anim;
        while(Time.time < duration)
        {
            InputManager.GetInputs();
            if(Time.time > min_crit && Time.time < max_crit)
            {
                if (InputManager.finisher && can_finisher)
                {
                    ExecuteEffect(ctx);
                    ExecuteFinisher(ctx);
                    yield break;
                }
            }
            else
            {
                if (InputManager.finisher) { can_finisher = false;}
            }
                yield return null;
        }
        ExecuteEffect(ctx);
    }
}
