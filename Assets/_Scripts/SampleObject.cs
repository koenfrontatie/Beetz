using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleObject : MonoBehaviour
{
    [SerializeField] public SampleInfo Info;

    private AudioSource _audioSource;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        if(Info.ID.Length < 5)   _audioSource.clip = SampleManager.Instance.BaseSamples[Info.Template];
        // TODO: else find unique clip
    }

    public void PlayAudio()
    {
        _audioSource.Play();
    }
}

