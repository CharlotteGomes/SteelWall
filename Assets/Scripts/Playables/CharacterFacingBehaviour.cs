using UnityEngine;
using UnityEngine.Playables;

// A behaviour that is attached to a playable
public class CharacterFacingBehaviour : PlayableBehaviour
{
    public Camera cam;
    public Transform tf;
    public Vector2 scale;
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
        tf.localScale = scale;
    }

    // Called when the state of the playable is set to Paused
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        if (playable.GetTime() >= playable.GetDuration())
        {
            
        }
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        float angle = cam.transform.eulerAngles.z;
        tf.localRotation = Quaternion.Euler(0, 0, angle);
    }

    // Called each frame while the state is set to Play
    public override void PrepareFrame(Playable playable, FrameData info)
    {
        
    }
}
