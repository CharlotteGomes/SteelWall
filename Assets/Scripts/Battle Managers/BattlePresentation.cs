using UnityEngine;
using UnityEngine.Playables;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
public static class BattlePresentation
{
    public static PlayableGraph graph { get; private set; } = PlayableGraph.Create();
    static IList<BuildPresentation> presentation_list = new List<BuildPresentation>(); 
    public static void CreateGraph() 
    {
        if(graph.IsValid()) graph.Destroy();
        graph = PlayableGraph.Create();
    }

    public static ScriptPlayable<PresentationStageBehaviour> SetStageAsBehaviour(PresentationStage stage)
    {
        var playable = ScriptPlayable<PresentationStageBehaviour>.Create(graph, stage.containers.Count);
        var behaviour = playable.GetBehaviour();
        double duration = 0;
       for(int i = 0; i < stage.containers.Count; i++)
        {
            graph.Connect(stage.containers[i].playable, 0, playable, i);
            behaviour.startTimes.Add(stage.containers[i].start_time);
            duration = duration > stage.containers[i].end_time ? duration : stage.containers[i].end_time;
            //Debug.Log($"Duração de playable {stage.containers[i].playable.ToString()}: {stage.containers[i].playable.GetDuration()}");
        }
       playable.SetDuration(duration);
       return playable;
    }

    public static ScriptPlayable<PresentationSequenceManager> BuildPresentationScene(Presentation presentation)
    {
        int input_count = presentation.Stages.Count;
        var playable = ScriptPlayable<PresentationSequenceManager>.Create(graph, input_count);
        var behaviour = playable.GetBehaviour();
        double duration = 0;
        double startTime = 0;
        for (int i = 0; i < input_count; i++) 
        {
            var stage = SetStageAsBehaviour(presentation.Stages[i]);
            graph.Connect(stage, 0, playable, i);
            double dur = stage.GetDuration();
            behaviour.startTimes.Add(startTime);
            startTime += dur;
            duration += dur; 
        }
        
        playable.SetDuration(duration);
        return playable;
    }

    #region Creates
    public static ScriptPlayable<CharacterFacingBehaviour> CreateFacing(float duration, Transform tf)
    {
        var playable = ScriptPlayable<CharacterFacingBehaviour>.Create(graph);
        var behaviour = playable.GetBehaviour();
        playable.SetDuration(duration);
        behaviour.cam = Camera.main;
        behaviour.tf = tf;
        return playable;
    }

    public static ScriptPlayable<CharacterMoveBehaviour> CreateMove(float duration, Transform tf, Vector2 start_pos, Vector2 target_pos)
    {
        var playable = ScriptPlayable<CharacterMoveBehaviour>.Create(graph);
        var behaviour = playable.GetBehaviour();
        playable.SetDuration(duration);
        behaviour.start_pos = start_pos;
        behaviour.target_pos = target_pos;
        behaviour.tf = tf;
        return playable;
    }

    public static ScriptPlayable<CameraZoomBehaviour> CreateZoom(float duration, float start_size, float end_size, float start_progress = 0)
    {
        var playable = ScriptPlayable<CameraZoomBehaviour>.Create(graph);
        var behaviour = playable.GetBehaviour();
        playable.SetDuration(duration);
        behaviour.cam = Camera.main;
        behaviour.start_size = start_size;
        behaviour.end_size = end_size;
        behaviour.start_progress = start_progress;
        return playable;
    }

    public static ScriptPlayable<CameraRotateBehaviour> CreateRotate(float duration, float start_rotate, float end_rotate, float start_progress = 0) 
    {
        var playable = ScriptPlayable<CameraRotateBehaviour>.Create(graph);
        var behaviour = playable.GetBehaviour();
        playable.SetDuration(duration);
        behaviour.cam = Camera.main;
        behaviour.start_rotate = start_rotate;
        behaviour.end_rotate = end_rotate;
        behaviour.start_progress = start_progress;
        return playable;
    }

    public static ScriptPlayable<CharacterChangeSpriteBehaviour> CreateSprite(SpriteRenderer spriteRenderer, Sprite spr)
    {
        var playable = ScriptPlayable<CharacterChangeSpriteBehaviour>.Create(graph);
        var behaviour = playable.GetBehaviour();
        playable.SetDuration(0.1f);
        behaviour.spriteRenderer = spriteRenderer;
        behaviour.spr = spr;
        return playable;
    }

    public static ScriptPlayable<CharacterReturn> CreateReturn(Transform tf, CharacterPresentationState state, SpriteRenderer spr_render, int order, Sprite spr)
    {
        var playable = ScriptPlayable<CharacterReturn>.Create(graph);
        var behaviour = playable.GetBehaviour();
        playable.SetDuration(0.3f);
        behaviour.tf = tf;
        behaviour.state = state;
        behaviour.spr_render = spr_render;
        behaviour.sortingOrder = order;
        behaviour.spr = spr;
        return playable;
    }

    public static ScriptPlayable<CharacterScale> CreateCharacterScale(float duration, Transform tf, Vector2 start_scaling, Vector2 end_scaling, float start_progress = 0)
    {
        var playable = ScriptPlayable<CharacterScale>.Create(graph);
        var behaviour = playable.GetBehaviour();
        playable.SetDuration(duration);
        behaviour.tf = tf;
        behaviour.start_scale = start_scaling;
        behaviour.start_progress = start_progress;
        behaviour.end_scale = end_scaling;
        return playable;
    }
    #endregion

    public static void AddBuild(BuildPresentation b)
    {
        presentation_list.Add(b);
    }

    public static Sprite SetSprites(PresentationSO so, BattleChar b)
    {
        return so.self switch
        {
            BattleSprites.Hurt => b.spriteSO.hurt,
            BattleSprites.Idle => b.spriteSO.idle,
            BattleSprites.buff => b.spriteSO.buff,
            _ => null
        };
    }
}
