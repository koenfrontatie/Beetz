using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioController : MonoBehaviour
{
    private AudioSource source;
    void Start()
    {
        source = GetComponent<AudioSource>();
        Hotbar.OnHotbarClicked += PlayHotbarSound;
    }

    void PlayClip(AudioClip clip)
    {
        source.PlayOneShot(clip);
    }

    void PlayHotbarSound(int i)
    {
        PlayClip(SampleManager.Instance.BaseSamples[i]);
    }
}
