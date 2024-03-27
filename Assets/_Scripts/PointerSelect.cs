using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointerSelect : MonoBehaviour, IPointerDownHandler
{
    SampleObject obj;

    void Start()
    {
        obj = GetComponent<SampleObject>();  
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SampleManager.Instance.SetSelectedTemplateSample(obj);
    }
}
