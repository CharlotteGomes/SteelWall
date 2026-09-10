using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;

public class BattleResult
{
    public List<BuildPresentation> buildPresentationList = new();
    public List<BattleChar> dodge = new List<BattleChar>();
    public List<BattleChar> hit = new List<BattleChar>();
    public int acc_base;
    public int acc_roll;
    public bool skill_fail = true;
    public bool crit = false;
    public void AddBuild(BuildPresentation b)
    {
        buildPresentationList.Add(b);
    }

    public ScriptPlayable<PresentationSequenceManager> BuildFinalResult()
    {
        Presentation presentation = new Presentation();
        foreach(var item in buildPresentationList)
        {
            var stage = item.BuildStageFromContext();
            presentation.AddStage(stage);
        }
        return BattlePresentation.BuildPresentationScene(presentation);
    }

}
