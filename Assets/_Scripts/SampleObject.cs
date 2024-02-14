using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleObject : MonoBehaviour
{
    [SerializeField] SampleInfo sampleInfo;

    void Start()
    {
        
    }

    
    void Update()
    {
        
    }
}

[System.Serializable]
public struct SampleInfo
{
    public int template;
    public string id;
    public string name;
    public string url;
}
