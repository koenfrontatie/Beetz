using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource src;
    [SerializeField] private AudioClip clip;
    [SerializeField] private AudioClip stepClip;

    private void Awake()
    {
        Metronome.NewBeat += PlayClip;
        Metronome.NewStep += PlayStep;
    }

    private void Start()
    {
        src = GetComponent<AudioSource>();
    }

    public void PlayClip()
    {
        src.PlayOneShot(clip);
    }

    public void PlayStep()
    {
        src.PlayOneShot(stepClip);
    }
}
