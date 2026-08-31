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
        Debug.Log("START PRESENTATION");

        graph = PlayableGraph.Create();

        List<BattleChar> chars = new();
        List<Vector2> original_positions = new();

        SetParticipants(chars, User, Targets);
        SetOriginalPositions(chars, original_positions);

        FirstAct(chars, so);

        Debug.Log($"Graph valid: {graph.IsValid()}");

        graph.Play();

        Debug.Log($"Graph playing: {graph.IsPlaying()}");

        yield return new WaitForSeconds(3);

        if (graph.IsValid())
        {
            graph.Destroy();
            Debug.Log("Deletou o graph");
        }
    }
    void SetParticipants(List<BattleChar> participants, BattleChar user, List<BattleChar> targets)
    {
        Debug.Log($"User: Settado {user.name}");
        participants.Add(user);
        if(targets.Count == 0) { return; }
        foreach (BattleChar c in targets){
            Debug.Log($"Target: Settado {c.name}");
            participants.Add(c);
        }
    }

    void SetOriginalPositions(List<BattleChar> ch, List<Vector2> pos)
    {
        foreach (BattleChar c in ch)
        {
            Debug.Log($"Original Position: Settado {c.name}");
            pos.Add(c.transform.localPosition);
        }
    }

    void FirstAct(List<BattleChar> c, PresentationSO so)
    {

        int i = 0;
        List<Playable> bh = new List<Playable>();
        List<Vector2> all_end_pos = new List<Vector2>();
        List<double> delays = new List<double>();
        all_end_pos.Add(so.self_end_pos);

        foreach (Vector2 v in so.targets_end_pos)
        {
            all_end_pos.Add(v);
        }

        foreach (BattleChar ch in c)
        {
            Transform tf = ch.GetComponent<Transform>();
            Vector2 start = tf.localPosition;
            Vector2 end = all_end_pos[i];
            bh.Add(CreateMove(2, tf, start, end));
            bh.Add(CreateFacing(2, tf, 1.5f));
            delays.Add(0);
            delays.Add(0);
            i++;
        }
        bh.Add(CreateZoom(.1f, 7f, 5.3f, 0.33f));
        bh.Add(CreateRotate(2f, 0, 5, 0.33f));
        bh.Add(SecondAct(c, so));
        delays.Add(0);
        delays.Add(0);
        delays.Add(2);
        var mixer = CreateMixer(bh, delays, bh.Count);
        var output = ScriptPlayableOutput.Create(graph, "first");
        output.SetSourcePlayable(mixer);
    }

    ScriptPlayable<PlayableMixerBehaviour> SecondAct(List<BattleChar> c, PresentationSO so)
    {

        int i = 0;
        List<Playable> bh = new List<Playable>();
        List<double> delays = new List<double>();


        foreach (BattleChar ch in c)
        {
            Transform tf = ch.GetComponent<Transform>();
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
