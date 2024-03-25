using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleObject : MonoBehaviour
{
    [SerializeField] public SampleInfo Info;

    [SerializeField] public SampleData SampleData;

    void Start()
    {
        if (Info.ID.Length < 5)
        {
            Info.ID = Info.Template.ToString();
        }

        if (SampleData.ID.Length < 5)
        {
            SampleData.ID = Info.Template.ToString();
        }
        // TODO: else find unique clip
    }
}

