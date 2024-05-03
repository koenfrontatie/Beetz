using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Toolbar : MonoBehaviour
{
    //[SerializeField] Transform _TBC;
    [SerializeField] private List<InventorySlot> _inventorySlots = new List<InventorySlot>();
    private List<SampleObject> _toolbarSampleObjects= new List<SampleObject>();

    private void OnEnable()
    {
        Events.ProjectDataLoaded += OnProjectDataLoaded;
        Events.SampleSelected += OnSampleSelected;
        Events.DragDropFoundNewContainer += OnDragDropFoundNewContainer;
    }

    private void OnDragDropFoundNewContainer(DragDropUI dragdrop, InventorySlot slot)
    {
        InventorySlot oldSlot = null;

        for(int i = 0; i < _inventorySlots.Count; i++)
        {
            if (_inventorySlots[i].InventoryDragDropUI == dragdrop)
            {
                oldSlot = _inventorySlots[i];
                break;
            }
        }

        // swap 

        DragDropUI dragDropToSwapWith = slot.InventoryDragDropUI;

        if(dragDropToSwapWith != null && oldSlot != null)
        {
            oldSlot.Bind(dragDropToSwapWith);
        }

        slot.Bind(dragdrop);

        UpdateToolbarConfiguration();
    }

    void UpdateToolbarConfiguration()
    {
        var data = new List<string>();

        for(int i = 0; i < _inventorySlots.Count; i++)
        {
            if (_inventorySlots[i].InventoryDragDropUI.gameObject.TryGetComponent<SampleObject>(out var so))
            {
                data.Add(so.SampleData.ID);
                _toolbarSampleObjects.Insert(i, so);
            } else
            {
                data.Add("-1");
            }
        }

        DataStorage.Instance.ProjectData.ToolbarConfiguration.IDC = data;
    }

    //private void Start()
    //{
    //    int slotCount = 0;

    //    for (int i = 0; i < _TBC.childCount; i++)
    //    {
    //        if (_TBC.GetChild(i).GetChild(0).TryGetComponent<InventorySlot>(out var slot))
    //        {
    //            _inventorySlots.Insert(slotCount, slot);
    //            slotCount++;
    //        } else
    //        {
    //            continue;
    //        }
    //    }
    //}

    void OnProjectDataLoaded(ProjectData projectData)
    {
        // clear existing items
        if (_toolbarSampleObjects.Count != 0)
        {
            foreach(var so in _toolbarSampleObjects)
            {
                Destroy(so.gameObject);
            }

            _toolbarSampleObjects.Clear();
        }
        // get project toolbar data
        var copy = new List<string>(projectData.ToolbarConfiguration.IDC);
       
        AssignItems(copy);

        SetToolbarItemsTransparent();
    }

    public void OnRefreshToolbar()
    {
        if (DataStorage.Instance.ProjectData.ToolbarConfiguration.IDC.Count == 0) return;
        OnProjectDataLoaded(DataStorage.Instance.ProjectData);
    }

    public void AssignItems(List<string> library)
    {
        if (library.Count != 0)
        {
            for (int i = 0; i < library.Count; i++)
            {
                if (i >= _inventorySlots.Count) return;

                if (library[i] == "-1") { continue; }
                
                var item = AssetBuilder.Instance.GetToolbarItem(library[i]);

                if (item.transform.TryGetComponent<DragDropUI>(out var dragdrop))
                {
                    _inventorySlots[i].Bind(dragdrop);
                }

                if(item.transform.TryGetComponent<SampleObject>(out var so))
                {
                    _toolbarSampleObjects.Insert(i, so);
                }

                item.transform.localScale = Vector3.one;
            }
        }
    }



    //void OnItemSwap(InventorySlot targetSlot, DragDropUI one, DragDropUI two)
    //{
    //    if (!one.transform.TryGetComponent<SampleObject>(out var oneSO)) return;

    //    int oldIndexOne = Array.IndexOf(_toolbarSampleObjects, oneSO);

    //    if(two == null)
    //    {
    //        int targetSlotIndex = Array.IndexOf(_containers, targetSlot);

    //        _toolbarSampleObjects[targetSlotIndex] = oneSO;
    //        _containers[targetSlotIndex].Bind(oneSO);
    //        _toolbarSampleObjects[oldIndexOne] = null;
    //        _containers[oldIndexOne].Bind(null);

    //        return;
    //    }

    //    if (!two.transform.TryGetComponent<SampleObject>(out var twoSO)) return;

    //    int oldIndexTwo = Array.IndexOf(_toolbarSampleObjects, twoSO);

    //    _toolbarSampleObjects[oldIndexOne] = twoSO;
    //    _containers[oldIndexOne].Bind(twoSO);
    //    _toolbarSampleObjects[oldIndexTwo] = oneSO;
    //    _containers[oldIndexTwo].Bind(oneSO);
    //}

    //void OnItemSwap(InventorySlot targetSlot, DragDropUI one, DragDropUI two)
    //{
    //    if (!one.transform.TryGetComponent<SampleObject>(out var oneSO)) return;

    //    int oldIndexOne = Array.IndexOf(_toolbarSampleObjects, oneSO);

    //    int targetSlotIndex = Array.IndexOf(_containers, targetSlot);
    //    if (targetSlotIndex < 0 || targetSlotIndex >= _containers.Length)
    //    {
    //        Debug.LogError("Target index is out of bounds.");
    //        return;
    //    }

    //    if (two == null)
    //    {
    //        _toolbarSampleObjects[targetSlotIndex] = oneSO;
    //        _containers[targetSlotIndex].Bind(oneSO);
    //        _toolbarSampleObjects[oldIndexOne] = null;
    //        _containers[oldIndexOne].Bind(null);
    //        return;
    //    }

    //    if (!two.transform.TryGetComponent<SampleObject>(out var twoSO)) return;

    //    int oldIndexTwo = Array.IndexOf(_toolbarSampleObjects, twoSO);
    //    if (oldIndexTwo < 0 || oldIndexTwo >= _toolbarSampleObjects.Length)
    //    {
    //        Debug.LogError("SampleObject index is out of bounds.");
    //        return;
    //    }

    //    _toolbarSampleObjects[oldIndexOne] = twoSO;
    //    _containers[oldIndexOne].Bind(twoSO);
    //    _toolbarSampleObjects[oldIndexTwo] = oneSO;
    //    _containers[oldIndexTwo].Bind(oneSO);
    //}

    void OnSampleSelected(SampleObject sampleObject)
    {
        for(int i = 0; i < _toolbarSampleObjects.Count; i++) {

            if (_toolbarSampleObjects[i] == null) continue;

            RawImage img;

            if (!_toolbarSampleObjects[i].TryGetComponent<RawImage>(out img))
            {
                continue;
            }

            if (_toolbarSampleObjects[i].SampleData.ID == sampleObject.SampleData.ID && img != null)
            {
                img.CrossFadeAlpha(1f, .1f, false);
            } else
            {
                img.CrossFadeAlpha(.5f, .1f, false);
            }
        
        }
    }

    void SetToolbarItemsTransparent()
    {
        for (int i = 0; i < _toolbarSampleObjects.Count; i++)
        {

            if (_toolbarSampleObjects[i] == null) continue;

            RawImage img;

            if (!_toolbarSampleObjects[i].TryGetComponent<RawImage>(out img))
            {
                continue;
            }

            img.CrossFadeAlpha(.5f, .1f, false);

        }
    }
    private void OnDisable()
    {
        Events.ProjectDataLoaded -= OnProjectDataLoaded;
        //Events.ItemSwap -= OnItemSwap;
        Events.DragDropFoundNewContainer -= OnDragDropFoundNewContainer;

        Events.SampleSelected -= OnSampleSelected;
    }
}
