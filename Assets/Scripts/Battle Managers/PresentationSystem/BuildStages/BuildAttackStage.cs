using UnityEngine;

public class BuildAttackStage : BuildPresentation
{
    public BuildAttackStage(PresentationContext ctx) : base(ctx)
    {
    }

    public override PresentationStage BuildStageFromContext()
    {
        PresentationStage stage = new PresentationStage();
        var user_facing = BattlePresentation.CreateFacing(2, context.userState.battleChar.tf);
        var user_move = BattlePresentation.CreateMove(2, context.userState.tf, (Vector2)context.userState.localPosition + Vector2.right * context.presentationSO.self_start_pos, Vector2.right * context.presentationSO.self_end_pos);
        var user_scale = BattlePresentation.CreateCharacterScale(0.1f, context.userState.tf, context.userState.localScale, Vector2.one * 1.5f, 1f);
        var user_sprite = BattlePresentation.CreateSprite(context.userState.spr_render, context.presentationSO.spr);
        var camera_zoom = BattlePresentation.CreateZoom(.1f, 7, 5.3f, .33f);
        var camera_rotate = BattlePresentation.CreateRotate(2f, 0, 5, 0.33f);

        stage.Add(user_facing, 0);
        stage.Add(user_move, 0);
        stage.Add(user_scale, 0);
        stage.Add(user_sprite, 0f);
        stage.Add(camera_zoom, 0f);
        stage.Add(camera_rotate, 0f);
        foreach(CharacterPresentationState n in context.targetState)
        {
            var facing = BattlePresentation.CreateFacing(2, n.tf);
            var move = BattlePresentation.CreateMove(2, n.tf, (Vector2)n.localPosition + Vector2.right * context.presentationSO.targets_start_pos, Vector2.right * context.presentationSO.targets_end_pos);
            var scale = BattlePresentation.CreateCharacterScale(0.1f, n.tf, n.localScale, Vector2.one * 1.5f, 1);
            var sprite = BattlePresentation.CreateSprite(n.spr_render, n.battleChar.spriteSO.hurt);
            stage.Add(facing, 0);
            stage.Add(move, 0);
            stage.Add(scale, 0);
            stage.Add(sprite, 0f);
        }

        return stage;
    }
}
