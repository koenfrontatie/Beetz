using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prefabs : MonoBehaviour
{
    public static Prefabs Instance;

    public Sequencer Sequencer;
    public Step Step;

    public Material White;
    public Material Blue;
    public Material DarkBlue;

    public List<SampleObject> BaseObjects = new List<SampleObject>();


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
}
