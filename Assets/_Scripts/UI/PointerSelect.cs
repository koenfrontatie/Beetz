using FileManagement;
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
        FileManager.SelectNewSample?.Invoke(obj.SampleData.ID);
        Events.LoadPlaySample?.Invoke(obj.SampleData);

        //for(int i = 0; i < obj.SampleData.Effects.Count; i++)
        //{
        //    Debug.Log($"{obj.SampleData.Effects[i].Effect.ToString()} : {obj.SampleData.Effects[i].Value.ToString()}");
        //}
        //Debug.Log(obj.SampleData.Effects.ToString());
    }
}
