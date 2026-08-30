using Mono.Cecil;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;
using TMPro;
public class BattlePresentation : MonoBehaviour
{
    PlayableGraph graph;
    [SerializeField] Camera cam;
    [SerializeField] TMP_Text text;
    public IEnumerator StartPresentation(BattleChar pj, BattleChar enemy)
    {
        graph = PlayableGraph.Create();

        SpriteRenderer pj_render = pj.GetComponent<SpriteRenderer>();
        SpriteRenderer enemy_render = enemy.GetComponent<SpriteRenderer>();
        Sprite pj_idle = pj_render.sprite;
        Sprite enemy_idle = enemy_render.sprite;
        Transform pj_transform = pj.GetComponent<Transform>();
        Transform enemy_transform = enemy.GetComponent<Transform>();
        Vector2 pj_start_pos = pj_transform.position;
        Vector2 enemy_start_pos = enemy_transform.position;
        Vector2 pj_end_pos = Vector2.right;
        Vector2 enemy_end_pos = Vector2.right * 4;
        
        var pj_facing = CreateFacing(2, pj_transform, 1.5f);
        var enemy_facing = CreateFacing(2, enemy_transform, 1.5f);

        var pj_move_in = CreateMove(2, pj_transform, Vector2.left * 3, pj_end_pos);
        var enemy_move_in = CreateMove(2, enemy_transform, Vector2.right * 3.75f, enemy_end_pos);

        var zoom_in = CreateZoom(0.1f, 7, 5.3f, .33f);
        var zoom_out = CreateZoom(0.2f, 5.3f, 7f);

        var rotate_in = CreateRotate(2, 0, 5, 0.33f);
        var rotate_out = CreateRotate(0.2f, 5, 0);

        var pj_move_out = CreateMove(0.2f, pj_transform, pj_end_pos, pj_start_pos);
        var enemy_move_out = CreateMove(0.2f, enemy_transform, enemy_end_pos, enemy_start_pos);

        var pj_facing_out = CreateFacing(0.2f, pj_transform, 1f);
        var enemy_facing_out = CreateFacing(0.2f, enemy_transform, 1f);

        var mixer_end = CreateMixer(new Playable[] { pj_move_out, enemy_move_out, pj_facing_out, enemy_facing_out, zoom_out, rotate_out}, new double[] { 0, 0, 0, 0, 0, 0}, 6);
        var mixer = CreateMixer(new Playable[] { pj_facing, enemy_facing, pj_move_in, enemy_move_in, zoom_in, rotate_in, mixer_end }, new double[] {0, 0, 0, 0, 0, 0, 2}, 7);
        var output = ScriptPlayableOutput.Create(graph, "first");
        output.SetSourcePlayable(mixer);
        graph.Play();


        yield return new WaitUntil(() => mixer.GetTime() >= mixer.GetDuration());
        if (graph.IsValid()) { graph.Destroy(); Debug.Log("Deletou o graph"); }
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
    ScriptPlayable<PlayableMixerBehaviour> CreateMixer(Playable[] inputs, double[] start_times, int inp)
    {
        if (inputs.Length != start_times.Length)
        {
            Debug.LogError("Inputs e start_times precisam ter o mesmo tamanho.");
            return default;
        }
        var mixer = ScriptPlayable<PlayableMixerBehaviour>.Create(graph, inputs.Length);
        var behaviour = mixer.GetBehaviour();
        behaviour.startTimes = start_times;
        behaviour.text = text;
        double duration = 0;
        for (int i = 0; i < inputs.Length; i++)
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
