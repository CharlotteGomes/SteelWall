using UnityEngine;
using UnityEngine.Playables;

// A behaviour that is attached to a playable
public class CharacterMoveBehaviour : PatternPlayable
{
    public Transform tf;
    public Vector2 start_pos;
    public Vector2 target_pos;
    public float start_process = 0;
    public Vector2 velocity = Vector2.zero;
    float smooth_time = 0;

    // Called when the owning graph stops playing
    public override void OnGraphStop(Playable playable)
    {

    }

    // Called when the state of the playable is set to Play
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        smooth_time = (float)playable.GetDuration();
    }

    // Called when the state of the playable is set to Paused
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (playable.GetTime() < start_time) { return; }
        tf.localPosition = Vector2.SmoothDamp(tf.localPosition, target_pos, ref velocity, smooth_time);
    }

    // Called each frame while the state is set to Play
    public override void PrepareFrame(Playable playable, FrameData info)
    {
        
    }
}
