using UnityEngine;
using UnityEngine.Playables;

// A behaviour that is attached to a playable
public class CameraZoomBehaviour : PlayableBehaviour
{
    public Camera cam;
    public float start_size = 0;
    public float end_size;
    public float start_progress = 0;
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

    // Called each frame while the state is set to Play
    public override void PrepareFrame(Playable playable, FrameData info)
    {
        
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        float t = (float)(playable.GetTime() / playable.GetDuration());
        float progress = Mathf.Lerp(start_progress, 1f, t);
        cam.orthographicSize = Mathf.Lerp(start_size, end_size, progress);
    }
}
