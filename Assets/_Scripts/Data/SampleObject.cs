using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleObject : MonoBehaviour
{
    [SerializeField] public SampleData SampleData;

    void Start()
    {

        if (SampleData.ID.Length < 5)
        {
            SampleData.ID = SampleData.Template.ToString();
        }
        // TODO: else find unique clip
    }
}

