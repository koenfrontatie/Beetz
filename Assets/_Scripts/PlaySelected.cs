using FileManagement;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlaySelected : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log(obj.SampleData.ID);
        //Events.SampleSelected?.Invoke(obj);
        Events.LoadPlayGuid?.Invoke(FileManager.Instance.SelectedSampleGuid);
        //Events.SetSelectedSample?.Invoke(obj);
        Events.SetSelectedGuid?.Invoke(FileManager.Instance.SelectedSampleGuid);

    }
}
