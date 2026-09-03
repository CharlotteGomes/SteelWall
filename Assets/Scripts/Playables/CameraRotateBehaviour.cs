using UnityEngine;
using UnityEngine.Playables;

// A behaviour that is attached to a playable
public class CameraRotateBehaviour : PatternPlayable
{
    public Camera cam;
    public float start_rotate = 0;
    public float end_rotate;
    public float start_progress;

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

    // Called each frame while the state is set to Play
    public override void PrepareFrame(Playable playable, FrameData info)
    {
        
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (playable.GetTime() < start_time) { return; }
        base.ProcessFrame(playable, info, playerData);
        float t = (float)((playable.GetTime() - start_time) / playable.GetDuration());
        float progress = Mathf.Lerp(start_progress, 1f, t);
        float angle = Mathf.Lerp(start_rotate, end_rotate, progress);
        cam.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
