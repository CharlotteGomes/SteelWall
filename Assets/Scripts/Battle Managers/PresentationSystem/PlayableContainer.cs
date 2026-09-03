using UnityEngine;
using UnityEngine.Playables;
public class PlayableContainer
{
    public Playable playable { get ; private set; }
    public double start_time { get; private set; } = 0;

    public double duration => playable.GetDuration();
    public double end_time => duration + start_time;
    public PlayableContainer(Playable p, double time)
    {
        playable = p;
        start_time = time;
    }
}
