using UnityEngine;

public abstract class BuildPresentation
{
    public PresentationContext context;
    public BuildPresentation(PresentationContext ctx) { context = ctx; }

    public abstract PresentationStage BuildStageFromContext();
}
