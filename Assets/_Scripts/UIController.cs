using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private Hotbar sampleHotbar;
    private void OnEnable()
    {
        //if (Input.GetKey(KeyCode.Tab)) 
            Events.OnBaseSamplesLoaded += SetHotbarText;
    }

    private void OnDisable()
    {
        //if (Input.GetKey(KeyCode.Tab)) 
        Events.OnBaseSamplesLoaded -= SetHotbarText;
    }
    void SetHotbarText()
    {
        var loader = FindObjectOfType<AndroidSampleFinder>();
        if (loader != null)
        {
            for (int i = 0; i < loader.BaseSampleNames.Count; i++)
            {
                sampleHotbar.SetButtonText(i, loader.BaseSampleNames[i]);
            }
        }
    }

}
