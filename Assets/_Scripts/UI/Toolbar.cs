using System.Collections.Generic;
using System;
using UnityEngine;

public class Toolbar : MonoBehaviour
{
    [SerializeField] Transform _TBC;
    [SerializeField] Transform _TBI;

    private InventorySlot[] _containers;
    private SampleObject[] _toolbarSampleObjects;

    [SerializeField] private ObjectMaker _objectMaker;

    private void OnEnable()
    {
        Events.ProjectDataLoaded += OnProjectDataLoaded;
        Events.ItemSwap += OnItemSwap;
    }

    private void Start()
    {
        _containers = new InventorySlot[Config.ToolbarCount]; // Ensure _containers is initialized properly
        _toolbarSampleObjects = new SampleObject[Config.ToolbarCount];

        for (int i = 0; i < _containers.Length; i++)
        {

            _containers[i] = _TBC.GetChild(i).GetComponent<InventorySlot>();
        }
    }

    public void AssignItems(List<string> library)
    {
        if (library.Count != 0)
        {
            for (int i = 0; i < library.Count; i++)
            {
                if (library[i] == "-1") { continue; }
                
                var item = _objectMaker.ToolbarItem(library[i]);
                
                item.transform.position = _containers[i].transform.position;

                item.transform.parent = _TBI;

                if (item.transform.TryGetComponent<SampleObject>(out var so))
                {
                    _containers[i].Bind(so);
                    _toolbarSampleObjects[i] = so;
                }
            }
        }
        Events.OnInventoryChange?.Invoke();
    }

    void OnProjectDataLoaded(ProjectData projectData)
    {
        var copy = new List<string>(projectData.IDCollection.IDC);
        AssignItems(copy);
    }

    void OnItemSwap(InventorySlot targetSlot, DragDropUI one, DragDropUI two)
    {
        if (!one.transform.TryGetComponent<SampleObject>(out var oneSO)) return;

        int oldIndexOne = Array.IndexOf(_toolbarSampleObjects, oneSO);
        
        if(two == null)
        {
            int targetSlotIndex = Array.IndexOf(_containers, targetSlot);
            
            _toolbarSampleObjects[targetSlotIndex] = oneSO;
            _containers[targetSlotIndex].Bind(oneSO);
            _toolbarSampleObjects[oldIndexOne] = null;
            _containers[oldIndexOne].Bind(null);

            return;
        }

        if (!two.transform.TryGetComponent<SampleObject>(out var twoSO)) return;

        int oldIndexTwo = Array.IndexOf(_toolbarSampleObjects, twoSO);

        _toolbarSampleObjects[oldIndexOne] = twoSO;
        _containers[oldIndexOne].Bind(twoSO);
        _toolbarSampleObjects[oldIndexTwo] = oneSO;
        _containers[oldIndexTwo].Bind(oneSO);
    }

    private void OnDisable()
    {
        Events.ProjectDataLoaded -= OnProjectDataLoaded;
        Events.ItemSwap -= OnItemSwap;
    }
}
