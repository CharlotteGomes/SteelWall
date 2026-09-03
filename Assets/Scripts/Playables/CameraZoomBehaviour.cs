using UnityEngine;
using UnityEngine.Playables;

// A behaviour that is attached to a playable
public class CameraZoomBehaviour : PatternPlayable
{
    public Camera cam;
    public float start_size = 0;
    public float end_size;
    public float start_progress = 0;
    public override void OnGraphStart(Playable playable)
    {
        base.OnGraphStart(playable);
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

    // Called each frame while the state is set to Play
    public override void PrepareFrame(Playable playable, FrameData info)
    {
        base.PrepareFrame(playable, info);
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (playable.GetTime() < start_time) { return; }
        float t = (float)((playable.GetTime() - start_time) / playable.GetDuration());
        float progress = Mathf.Lerp(start_progress, 1f, t);
        cam.orthographicSize = Mathf.Lerp(start_size, end_size, progress);
    }
}
