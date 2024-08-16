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
        //transform.position = pos;
        //Instantiate(_dirtParticleSystem, pos, transform.rotation);
        var psystem = Instantiate(_dirtParticleSystem, transform);
        psystem.transform.position = pos;
    }

    void OnDisable()
    {
        Events.SampleSpawned -= OnSampleSpawned;
    }
}
