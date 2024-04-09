
using UnityEngine;

public class PlaybackController : MonoBehaviour
{
    //public PlaybackMode PlaybackMode { get; private set; }
    public PlaybackMode PlaybackMode;
    public void TogglePlaybackMode() => PlaybackMode.NextEnumValue();
}

public enum PlaybackMode
{
    Linear,
    Circular
}