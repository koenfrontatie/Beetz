using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleObject : MonoBehaviour
{
    public struct SampleInfo
    {
        public string id;
        public string name;
        public string url;
        public BaseModel baseModel;
    }

    void Start()
    {
        
    }

    
    void Update()
    {
        
    }
}

public enum BaseModel
{
    Orange,
    Verdebloom
}
