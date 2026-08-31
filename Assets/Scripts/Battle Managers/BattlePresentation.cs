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
    BattleChar User;
    List<BattleChar> Targets = new();
    Sprite user_spr;
    List<Sprite> target_spr = new();
    public IEnumerator StartPresentation(BattleChar User, List<BattleChar> Targets, PresentationSO so)
    {
        Debug.Log("START PRESENTATION");

        graph = PlayableGraph.Create();

        this.User = User;
        this.Targets = Targets;
        SetSprites(so);
        var mixer = FirstAct(so);

        Debug.Log($"Graph valid: {graph.IsValid()}");

        graph.Play();

        Debug.Log($"Graph playing: {graph.IsPlaying()}");

        yield return new WaitForSeconds((float)mixer.GetDuration());

        if (graph.IsValid())
        {
            graph.Destroy();
            Debug.Log("Deletou o graph");
        }
    }

    ScriptPlayable<PlayableMixerBehaviour> FirstAct(PresentationSO so)
    {
        int i = 0;
        List<Playable> bh = new List<Playable>();
        List<double> delays = new List<double>();
        Transform user_tf = User.GetComponent<Transform>();
        SpriteRenderer user_sr = User.GetComponent<SpriteRenderer>();
        bh.Add(CreateMove(2, user_tf, so.self_start_pos, so.self_end_pos));
        bh.Add(CreateFacing(2, user_tf, 1.5f));
        bh.Add(CreateSprite(user_sr, user_spr));
        delays.Add(0);
        delays.Add(0);
        delays.Add(0);
        foreach (BattleChar ch in Targets)
        {
            Transform tf = ch.GetComponent<Transform>();
            SpriteRenderer sprRender = ch.GetComponent<SpriteRenderer>();
            Vector2 start = tf.localPosition;
            Vector2 end = so.targets_end_pos;
            bh.Add(CreateMove(2, tf, start, end));
            bh.Add(CreateFacing(2, tf, 1.5f));
            bh.Add(CreateSprite(sprRender, target_spr[0]));
            delays.Add(0);
            delays.Add(0);
            delays.Add(0);
            i++;
        }
        bh.Add(CreateZoom(.1f, 7f, 5.3f, 0.33f));
        bh.Add(CreateRotate(2f, 0, 5, 0.33f));
        bh.Add(SecondAct(so));
        delays.Add(0);
        delays.Add(0);
        delays.Add(2);
        var mixer = CreateMixer(bh, delays, bh.Count);
        var output = ScriptPlayableOutput.Create(graph, "first");
        output.SetSourcePlayable(mixer);
        return mixer;
    }

    ScriptPlayable<PlayableMixerBehaviour> SecondAct(PresentationSO so)
    {
        int i = 0;
        List<Playable> bh = new List<Playable>();
        List<double> delays = new List<double>();
        Transform user_tf = User.GetComponent<Transform>();
        SpriteRenderer user_sr = User.GetComponent<SpriteRenderer>();
        bh.Add(CreateReturn(0.3f, 1.5f, user_tf.transform.localPosition, user_tf));
        bh.Add(CreateSprite(user_sr, User.spriteSO.idle));
        delays.Add(0);
        delays.Add(0.4f);

        foreach (BattleChar ch in Targets)
        {
            Transform tf = ch.GetComponent<Transform>();
            SpriteRenderer sprRender = ch.GetComponent<SpriteRenderer>();
            bh.Add(CreateSprite(sprRender, Targets[i].spriteSO.idle));
            delays.Add(0.4f);
            bh.Add(CreateReturn(0.3f, 1.5f, tf.transform.localPosition, tf));
            delays.Add(0f);
            i++;
        }
        bh.Add(CreateZoom(0.5f, 5.3f, 7f));
        bh.Add(CreateRotate(0.5f, 5, 0));
        delays.Add(0);
        delays.Add(0);
        return CreateMixer(bh, delays, bh.Count);
    }
    ScriptPlayable<CharacterFacingBehaviour> CreateFacing(float duration, Transform tf, float scale)
    {
        var playable = ScriptPlayable<CharacterFacingBehaviour>.Create(graph);
        var behaviour = playable.GetBehaviour();
        playable.SetDuration(duration);
        behaviour.cam = cam;
        behaviour.tf = tf;
        behaviour.scale = Vector2.one * scale;
        return playable;
    }

    ScriptPlayable<CharacterMoveBehaviour> CreateMove(float duration, Transform tf, Vector2 start_pos, Vector2 target_pos)
    {
        var playable = ScriptPlayable<CharacterMoveBehaviour>.Create(graph);
        var behaviour = playable.GetBehaviour();
        playable.SetDuration(duration);
        behaviour.start_pos = start_pos;
        behaviour.target_pos = target_pos;
        behaviour.tf = tf;
        return playable;
    }

    ScriptPlayable<CameraZoomBehaviour> CreateZoom(float duration, float start_size, float end_size, float start_progress = 0)
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

    ScriptPlayable<CameraRotateBehaviour> CreateRotate(float duration, float start_rotate, float end_rotate, float start_progress = 0) 
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

    ScriptPlayable<CharacterChangeSpriteBehaviour> CreateSprite(SpriteRenderer spriteRenderer, Sprite spr)
    {
        var playable = ScriptPlayable<CharacterChangeSpriteBehaviour>.Create(graph);
        var behaviour = playable.GetBehaviour();
        playable.SetDuration(0.1f);
        behaviour.spriteRenderer = spriteRenderer;
        behaviour.spr = spr;
        return playable;
    }

    ScriptPlayable<CharacterReturn> CreateReturn(float duration, float scale, Vector2 pos, Transform tf)
    {
        var playable = ScriptPlayable<CharacterReturn>.Create(graph);
        var behaviour = playable.GetBehaviour();
        playable.SetDuration(duration);
        behaviour.scale = scale;
        behaviour.tf = tf;
        behaviour.pos = pos;
        return playable; 
    }

    ScriptPlayable<PlayableMixerBehaviour> CreateMixer(List<Playable> inputs, List<double> start_times, int inp)
    {
        if (inputs.Count != start_times.Count)
        {
            Debug.LogError($"Inputs e start_times precisam ter o mesmo tamanho. {start_times.Count} | {inp}");
            return default;
        }
        var mixer = ScriptPlayable<PlayableMixerBehaviour>.Create(graph, inputs.Count);
        var behaviour = mixer.GetBehaviour();
        behaviour.startTimes = start_times;
        double duration = 0;
        for (int i = 0; i < inputs.Count; i++)
        {
            graph.Connect(inputs[i], 0, mixer, i);
            double endTime = start_times[i] + inputs[i].GetDuration();
            duration = System.Math.Max(duration, endTime);
        }
        mixer.SetDuration(duration);
        return mixer;
    }

    void SetSprites(PresentationSO so)
    {
        switch (so.self)
        {
            case BattleSprites.Slash:
                user_spr = User.spriteSO.slash;
                break;

            case BattleSprites.Pierce:
                user_spr = User.spriteSO.pierce;
                break;

            case BattleSprites.Bludgeoning:
                user_spr = User.spriteSO.bludgeoning;
                break;

            case BattleSprites.Casting:
                user_spr = User.spriteSO.casting;
                break;

            case BattleSprites.Ranged:
                user_spr = User.spriteSO.ranged;
                break;

            case BattleSprites.Hurt:
                user_spr = User.spriteSO.hurt;
                break;
        }

        foreach (BattleChar ch in Targets)
        {
            switch (so.targets)
            {
                case BattleSprites.Slash:
                    target_spr.Add(ch.spriteSO.slash);
                    break;

                case BattleSprites.Pierce:
                    target_spr.Add(ch.spriteSO.pierce);
                    break;

                case BattleSprites.Bludgeoning:
                    target_spr.Add(ch.spriteSO.bludgeoning);
                    break;

                case BattleSprites.Casting:
                    target_spr.Add(ch.spriteSO.casting);
                    break;

                case BattleSprites.Ranged:
                    target_spr.Add(ch.spriteSO.ranged);
                    break;

                case BattleSprites.Hurt:
                    target_spr.Add(ch.spriteSO.hurt);
                    break;
            }
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space)) { StartCoroutine(StartApresentation()); }
    }

    private void OnDestroy()
    {
        if (graph.IsValid()) graph.Destroy();
    }
}
