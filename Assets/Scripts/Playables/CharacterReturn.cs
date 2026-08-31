using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;

// A behaviour that is attached to a playable
public class CharacterReturn : PlayableBehaviour
{
    public Transform tf;
    public float scale;
    public Vector2 pos;
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
        float t = Mathf.Clamp01((float)(playable.GetTime() / playable.GetDuration()));
        float angle = Mathf.Lerp(5, 0, t);
        tf.localPosition = Vector2.Lerp(pos, Vector2.zero, t);
        tf.localScale = Vector2.Lerp(Vector2.one * scale, Vector3.one, t);
        tf.rotation = Quaternion.Euler(tf.rotation.eulerAngles.x, tf.rotation.eulerAngles.y, angle);
    }

    // Called each frame while the state is set to Play
    public override void PrepareFrame(Playable playable, FrameData info)
    {
        
    }
}
