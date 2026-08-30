using UnityEngine;
using UnityEngine.Playables;
using TMPro;
// A behaviour that is attached to a playable
public class PlayableMixerBehaviour : PlayableBehaviour
{
    public double[] startTimes;
    public TMP_Text text;
    public override void OnGraphStart(Playable playable)
    {
        for (int i = 0; i < playable.GetInputCount(); i++)
        {
            Playable input = playable.GetInput(i);
            input.Pause();
        }
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
        double mixerTime = playable.GetTime();
        text.text = $"{mixerTime}";
        for (int i = 0; i < playable.GetInputCount(); i++)
        {
            Playable input = playable.GetInput(i);

            if (!input.IsValid())
                continue;

            double startTime = startTimes[i];
            double inputDuration = input.GetDuration();

            if (mixerTime < startTime)
            {
                continue;
            }

            double inputTime = mixerTime - startTime;

            if (inputTime >= inputDuration)
            {
                input.Pause();
                continue;
            }
            input.Play();
            input.SetSpeed(1);
        }
    }

    // Called each frame while the state is set to Play
    public override void PrepareFrame(Playable playable, FrameData info)
    {
        
    }
}
