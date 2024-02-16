using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleObject : MonoBehaviour
{
    [SerializeField] public SampleInfo Info;

    public string ID;
    public string Name;
    public int Template;

    void Awake()
    {
        ID = Info.ID; Name = Info.Name; Template = Info.Template;
    }

    void Update()
    {
        
    }
}

