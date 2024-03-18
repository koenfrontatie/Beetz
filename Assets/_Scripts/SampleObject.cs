using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleObject : MonoBehaviour
{
    [SerializeField] public SampleInfo Info;

    void Start()
    {
        if (Info.ID.Length < 5) {
            Info.ID = Info.Template.ToString();
        }
        // TODO: else find unique clip
    }
}

