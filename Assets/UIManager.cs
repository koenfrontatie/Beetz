using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Hotbar sampleHotbar;
    private void Start()
    {
        //if (Input.GetKey(KeyCode.Tab)) 
            SetHotbarText();
    }
    void SetHotbarText()
    {
        var loader = FindObjectOfType<SampleLoader>();
        if (loader != null)
        {
            for (int i = 0; i < loader.BaseSamples.Count; i++)
            {
                sampleHotbar.SetButtonText(i, loader.BaseSamples[i].name);
            }
        }
    }

}
