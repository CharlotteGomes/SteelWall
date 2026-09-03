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
    public IEnumerator StartPresentation(BattleChar User, List<BattleChar> Targets, PresentationSO so)
    {
        yield return null;
        graph = PlayableGraph.Create();

        graph.Play();
        //yield return new WaitForSeconds((float)mixer.GetDuration());
        //IDEIA: FAZER UM VOID QUE ACEITA 3 MIXERS, UM SENDO OPCIONAL, DEPOIS ELE MONTA A TIMELINE E RODA
        if (graph.IsValid())
        {
            graph.Destroy();
        }
    }
    /*
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
    */
    public ScriptPlayable<CharacterFacingBehaviour> CreateFacing(float duration, Transform tf)
    {
        var playable = ScriptPlayable<CharacterFacingBehaviour>.Create(graph);
        var behaviour = playable.GetBehaviour();
        playable.SetDuration(duration);
        behaviour.cam = cam;
        behaviour.tf = tf;
        return playable;
    }

    public ScriptPlayable<CharacterMoveBehaviour> CreateMove(float duration, float delay, Transform tf, Vector2 start_pos, Vector2 target_pos)
    {
        var playable = ScriptPlayable<CharacterMoveBehaviour>.Create(graph);
        var behaviour = playable.GetBehaviour();
        playable.SetDuration(duration);
        behaviour.start_pos = start_pos;
        behaviour.target_pos = target_pos;
        behaviour.tf = tf;
        behaviour.start_time = delay;
        return playable;
    }

    public ScriptPlayable<CameraZoomBehaviour> CreateZoom(float duration, float delay, float start_size, float end_size, float start_progress = 0)
    {
        var playable = ScriptPlayable<CameraZoomBehaviour>.Create(graph);
        var behaviour = playable.GetBehaviour();
        playable.SetDuration(duration);
        behaviour.cam = cam;
        behaviour.start_size = start_size;
        behaviour.end_size = end_size;
        behaviour.start_progress = start_progress;
        behaviour.start_time = delay;
        return playable;
    }

    public ScriptPlayable<CameraRotateBehaviour> CreateRotate(float duration, float delay, float start_rotate, float end_rotate, float start_progress = 0) 
    {
        var playable = ScriptPlayable<CameraRotateBehaviour>.Create(graph);
        var behaviour = playable.GetBehaviour();
        playable.SetDuration(duration);
        behaviour.cam = cam;
        behaviour.start_rotate = start_rotate;
        behaviour.end_rotate = end_rotate;
        behaviour.start_time = delay;
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
    public ScriptPlayable<PlayableMixerBehaviour> CreateMixer(List<Playable> inputs, float delay)
    {
        var mixer = ScriptPlayable<PlayableMixerBehaviour>.Create(graph, inputs.Count);
        var behaviour = mixer.GetBehaviour();
        behaviour.start_time = delay;
        float duration = Mathf.Infinity;
        for (int i = 0; i < inputs.Count; i++)
        {
            var bh = ((ScriptPlayable<PatternPlayable>)inputs[i]).GetBehaviour();
            duration = Mathf.Max(duration, (float)inputs[i].GetDuration() + bh.start_time);
            graph.Connect(inputs[i], 0, mixer, i);
        }
        mixer.SetDuration(duration);
        return mixer;
    }

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
    }

    private void OnDestroy()
    {
        if (graph.IsValid()) graph.Destroy();
    }
}
