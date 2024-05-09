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
        //Debug.Log(obj.SampleData.ID);
        //Events.SampleSelected?.Invoke(obj);
        Events.LoadPlayGuid?.Invoke(obj.SampleData.ID);
        //Events.SetSelectedSample?.Invoke(obj);
        Events.SetSelectedGuid?.Invoke(obj.SampleData.ID);

    }
}
