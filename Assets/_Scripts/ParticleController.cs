using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    [SerializeField] private ParticleSystem _dirtParticleSystem;
    void Start()
    {
        Events.SampleSpawned += OnSampleSpawned;
    }

    void OnSampleSpawned(Vector3 pos)
    {
        transform.position = pos;

        _dirtParticleSystem.Play();
    }

    void OnDisable()
    {
        Events.SampleSpawned -= OnSampleSpawned;
    }
}
