using UnityEngine;
using UnityEngine.Playables;
using System.Collections.Generic;
// A behaviour that is attached to a playable
public class PresentationStageBehaviour : PlayableBehaviour
{
    public List<double> startTimes = new();
    public double self_start_time = 0;
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
        double stageTime = playable.GetTime();
        for (int i = 0; i < playable.GetInputCount(); i++)
        {
            var input = playable.GetInput(i);

            double start = startTimes[i];
            double duration = input.GetDuration();

            double end = start + duration;

            if (stageTime < start)
            {
                input.Pause();
                continue;
            }

            if (stageTime >= end)
            {
                input.Pause();
                continue;
            }

            // Está executando
            input.Play();

            playable.SetInputWeight(i, 1);
        }
    }
}

