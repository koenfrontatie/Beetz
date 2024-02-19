using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
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
        var loader = FindObjectOfType<SampleManager>();
        if (loader != null)
        {
            for (int i = 0; i < loader.BaseSamples.Count; i++)
            {
                sampleHotbar.SetButtonText(i, loader.BaseSamples[i].name);
            }
        }
    }

}
