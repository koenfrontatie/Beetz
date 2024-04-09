
using UnityEngine;

public class PlaybackController : MonoBehaviour
{
    public static PlaybackController Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    //public PlaybackMode PlaybackMode { get; private set; }
    public PlaybackMode PlaybackMode;
    public void TogglePlaybackMode() => PlaybackMode = PlaybackMode.NextEnumValue();
}

public enum PlaybackMode
{
    Linear,
    Circular
}