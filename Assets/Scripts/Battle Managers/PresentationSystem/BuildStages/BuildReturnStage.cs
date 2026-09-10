using UnityEngine;

public class BuildReturnStage : BuildPresentation
{
    public BuildReturnStage(PresentationContext ctx) : base(ctx) { }


    public override PresentationStage BuildStageFromContext()
    {
        PresentationStage stage = new PresentationStage();
        var user_return = BattlePresentation.CreateReturn(context.userState.tf, context.userState, context.userState.spr_render, context.userState.orderInLayer, context.userState.sprite);
        var user_scale = BattlePresentation.CreateCharacterScale(0.3f, context.userState.tf, Vector2.one * 1.5f, context.userState.localScale);
        var user_facing = BattlePresentation.CreateFacing(0.5f, context.userState.tf);
        var camera_zoom = BattlePresentation.CreateZoom(.3f, 5.3f, context.cameraState.ortographicSize, 0f);
        var camera_rotate = BattlePresentation.CreateRotate(.3f, 5, 0, 0.33f);
        stage.Add(user_return, 0);
        stage.Add(user_scale, 0);
        stage.Add(camera_zoom, 0f);
        stage.Add(camera_rotate, 0f); 
        stage.Add(user_facing, 0f);
        foreach(CharacterPresentationState n in context.targetState)
        {
            var target_return = BattlePresentation.CreateReturn(n.tf, n, n.spr_render, n.orderInLayer, n.sprite);
            var target_scale = BattlePresentation.CreateCharacterScale(0.3f, n.tf, Vector2.one * 1.5f, n.localScale);
            var target_facing = BattlePresentation.CreateFacing(.5f, n.tf);
            stage.Add(target_return, 0);
            stage.Add(target_facing, 0f);   
            stage.Add(target_scale, 0);
        }

        return stage;
    }
}
