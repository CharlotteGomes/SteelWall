using Mono.Cecil;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;
using System.Collections.Generic;
public class BattlePresentation : MonoBehaviour
{
    PlayableGraph graph;
    [SerializeField] Camera cam;
    [SerializeField] BattleChar test_attacker;
    [SerializeField] BattleChar[] test_defender;
    public IEnumerator StartPresentation()
    {
        yield return null;

        graph.Play();
        //yield return new WaitForSeconds((float)mixer.GetDuration());
        
        if (graph.IsValid())
        {
            graph.Destroy();
        }
    }

    public void CreateGraph() 
    {
        if(graph.IsValid()) graph.Destroy();
        graph = PlayableGraph.Create();
    }

    public ScriptPlayable<PresentationStageBehaviour> SetStageAsBehaviour(PresentationStage stage)
    {
        var playable = ScriptPlayable<PresentationStageBehaviour>.Create(graph, stage.containers.Count);
        var behaviour = playable.GetBehaviour();
        double duration = Mathf.Infinity;
       for(int i = 0; i < stage.containers.Count; i++)
        {
            graph.Connect(stage.containers[i].playable, 0, playable, i);
            behaviour.startTimes.Add(stage.containers[i].start_time);
            duration = duration > stage.containers[i].end_time ? duration : stage.containers[i].end_time;
        }
       playable.SetDuration(duration);
        return playable;
    }

    #region Creates
    public ScriptPlayable<CharacterFacingBehaviour> CreateFacing(float duration, Transform tf)
    {
        var playable = ScriptPlayable<CharacterFacingBehaviour>.Create(graph);
        var behaviour = playable.GetBehaviour();
        playable.SetDuration(duration);
        behaviour.cam = cam;
        behaviour.tf = tf;
        return playable;
    }

    public ScriptPlayable<CharacterMoveBehaviour> CreateMove(float duration, Transform tf, Vector2 start_pos, Vector2 target_pos)
    {
        var playable = ScriptPlayable<CharacterMoveBehaviour>.Create(graph);
        var behaviour = playable.GetBehaviour();
        playable.SetDuration(duration);
        behaviour.start_pos = start_pos;
        behaviour.target_pos = target_pos;
        behaviour.tf = tf;
        return playable;
    }

    public ScriptPlayable<CameraZoomBehaviour> CreateZoom(float duration, float start_size, float end_size, float start_progress = 0)
    {
        var playable = ScriptPlayable<CameraZoomBehaviour>.Create(graph);
        var behaviour = playable.GetBehaviour();
        playable.SetDuration(duration);
        behaviour.cam = cam;
        behaviour.start_size = start_size;
        behaviour.end_size = end_size;
        behaviour.start_progress = start_progress;
        return playable;
    }

    public ScriptPlayable<CameraRotateBehaviour> CreateRotate(float duration, float start_rotate, float end_rotate, float start_progress = 0) 
    {
        var playable = ScriptPlayable<CameraRotateBehaviour>.Create(graph);
        var behaviour = playable.GetBehaviour();
        playable.SetDuration(duration);
        behaviour.cam = cam;
        behaviour.start_rotate = start_rotate;
        behaviour.end_rotate = end_rotate;
        behaviour.start_progress = start_progress;
        return playable;
    }

    public ScriptPlayable<CharacterChangeSpriteBehaviour> CreateSprite(SpriteRenderer spriteRenderer, Sprite spr)
    {
        var playable = ScriptPlayable<CharacterChangeSpriteBehaviour>.Create(graph);
        var behaviour = playable.GetBehaviour();
        playable.SetDuration(0.1f);
        behaviour.spriteRenderer = spriteRenderer;
        behaviour.spr = spr;
        return playable;
    }

    #endregion

    public Sprite SetSprites(PresentationSO so, BattleChar b)
    {
        switch (so.self)
        {
            case BattleSprites.Slash:
                return b.spriteSO.slash;

            case BattleSprites.Pierce:
                return b.spriteSO.pierce;

            case BattleSprites.Bludgeoning:
                return b.spriteSO.bludgeoning;

            case BattleSprites.Casting:
                return b.spriteSO.casting;

            case BattleSprites.Ranged:
                return b.spriteSO.ranged;

            case BattleSprites.Hurt:
                return b.spriteSO.hurt;
            default:
                return null;
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CreateGraph();
            var zoom = CreateZoom(3, 7, 1);
            var rotate = CreateRotate(3, 0, 15, .33f);
            PresentationStage stage = new PresentationStage(2);
            stage.Add(zoom, 3);
            stage.Add(rotate, 0);
            var presentation = SetStageAsBehaviour(stage);
            var output = ScriptPlayableOutput.Create(graph, "presentation");
            output.SetSourcePlayable(presentation);
            graph.Play();
        }
    }

    private void OnDestroy()
    {
        if (graph.IsValid()) graph.Destroy();
    }
}
