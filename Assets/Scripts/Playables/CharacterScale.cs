using UnityEngine;
using UnityEngine.Playables;

// A behaviour that is attached to a playable
public class CharacterScale : PlayableBehaviour
{
    public Transform tf;
    public Vector2 start_scale;
    public Vector2 end_scale;
    public float start_progress;
    public override void OnGraphStart(Playable playable)
    {
        
    }

    // Called when the owning graph stops playing
    public override void OnGraphStop(Playable playable)
    {
        
    }

    // Called when the state of the playable is set to Play
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        
    }

    // Called when the state of the playable is set to Paused
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        float t = (float)(playable.GetTime() / playable.GetDuration());
        float progress = Mathf.Clamp01(Mathf.Lerp(start_progress, 1, t));
        tf.localScale = Vector2.Lerp(start_scale, end_scale, progress);
    }

    // Called each frame while the state is set to Play
    public override void PrepareFrame(Playable playable, FrameData info)
    {
        
    }
}
