using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragDropUI : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Vector3 _startPosition;
    [SerializeField] private bool _resetPositionOnRelease = true;
    [SerializeField] private Transform _parentWhileDragging;

    private void Start()
    {
        _parentWhileDragging = GameObject.FindWithTag("DragParent").transform;
    }
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_resetPositionOnRelease)
            _startPosition = transform.position;

        transform.SetParent(_parentWhileDragging);
        transform.SetAsLastSibling();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        var hits = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, hits);

        foreach (var hit in hits)
        {
            var droppedContainer = hit.gameObject.transform.GetComponent<InventorySlot>();
            
            if (droppedContainer)
            {
                //Debug.Log($"Dropped container is not null and is {droppedContainer.name}");
                
                _startPosition = droppedContainer.transform.position;  
                
                Events.DragDropFoundNewContainer?.Invoke(this, droppedContainer);
            }
        }

        if (_resetPositionOnRelease)
            transform.position = _startPosition;
    }
}
