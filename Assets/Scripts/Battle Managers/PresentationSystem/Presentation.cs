using UnityEngine;
using System.Collections.Generic;
public class Presentation
{
    List<PresentationStage> stages = new List<PresentationStage>();
    public IReadOnlyList<PresentationStage> Stages => stages;
    public Presentation AddStage(PresentationStage s)
    {
        stages.Add(s);
        return this;
    }
}
