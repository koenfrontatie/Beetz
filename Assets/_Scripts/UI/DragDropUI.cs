using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragDropUI : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Vector3 _startPosition;
    private Transform _startParent;
    [SerializeField] private bool _resetPositionOnRelease = true;
    public bool isLibraryItem;
    [SerializeField] private Transform _parentWhileDragging;

    private void Start()
    {
        _parentWhileDragging = GameObject.FindWithTag("DragParent").transform;
    }
    public void OnDrag(PointerEventData eventData)
    {
        //transform.position = eventData.position;

        var screenPoint = (Vector3)eventData.position;
        screenPoint.z = 10.0f; //distance of the plane from the camera
        transform.position = Prefabs.Instance.CanvasCamera.ScreenToWorldPoint(screenPoint);

    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_resetPositionOnRelease)
        {
            _startPosition = transform.position;
            _startParent = transform.parent;
        }


        foreach (Transform child in transform.parent)
        {
            if(child.TryGetComponent<LibrarySlot>(out var slot))
            {
                isLibraryItem = true;
                
                var copy = Instantiate(this.gameObject, slot.transform.position, Quaternion.identity, slot.transform.parent);
                copy.name = this.gameObject.name;

            } 
            
            if(child.TryGetComponent<InventorySlot>(out var slot2))
            {
                isLibraryItem = false;
            }
        }

        //Debug.Log(isLibraryItem);


        transform.SetParent(_parentWhileDragging);
        transform.SetAsLastSibling();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        var hits = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, hits);
        bool foundInventorySlot = false;
        
        foreach (var hit in hits)
        {
            if (hit.gameObject.transform.TryGetComponent<InventorySlot>(out var droppedOninventory))
            {

                _startPosition = droppedOninventory.transform.position;

                Events.DragDropFoundNewContainer?.Invoke(this, droppedOninventory);

                foundInventorySlot = true;

                Toolbar.Instance.OnDragDropFoundNewContainer(this, droppedOninventory);

                //if (_resetPositionOnRelease)
                //    transform.position = _startPosition;

            }
        }



        //Debug.Log($"found ivnentory: {foundInventorySlot}");

        if(!foundInventorySlot)
        {
            if (isLibraryItem)
            {
                Toolbar.Instance.OnDragDropFoundNewContainer(this, null);

            }
            else
            {
                if (_resetPositionOnRelease)
                {
                    transform.position = _startPosition;
                    transform.SetParent(_startParent);
                }
            }
        }
    }
 }

