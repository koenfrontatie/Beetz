using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOVWatcher : MonoBehaviour
{
    [SerializeField]
    private Camera fovCam, main;

    float lastfov;


    // Update is called once per frame
    void Update()
    {
        if (lastfov != main.fieldOfView)
        {
            fovCam.fieldOfView = main.fieldOfView;
        }
        lastfov = main.fieldOfView;        
    }
}
