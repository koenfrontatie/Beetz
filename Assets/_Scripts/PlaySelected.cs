using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlaySelected : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log(obj.SampleData.ID);
        //Events.SampleSelected?.Invoke(obj);
        Events.LoadPlayGuid?.Invoke(AssetBuilder.Instance.SelectedGuid);
        //Events.SetSelectedSample?.Invoke(obj);
        Events.SetSelectedGuid?.Invoke(AssetBuilder.Instance.SelectedGuid);

    }
}
