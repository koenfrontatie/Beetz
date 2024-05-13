using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using FileManagement;

public class Toolbar : MonoBehaviour
{
    [SerializeField] private List<InventorySlot> _inventorySlots = new List<InventorySlot>();
    //private List<string> _toolbarSampleCollection= new List<string>();
    [SerializeField]
    private IDCollection _toolbarSampleCollection;
    [SerializeField]
    private List<GameObject> _toolbarButtons = new List<GameObject>();
    public static Toolbar Instance;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void OnEnable()
    {
        DataStorage.ProjectDataSet += OnProjectDataLoaded;
        FileManager.NewSampleSelected += SetToolbarSelection;
        Events.DragDropFoundNewContainer += OnDragDropFoundNewContainer;
        GameManager.StateChanged += OnNewGameMode;
    }

    public void OnDragDropFoundNewContainer(DragDropUI dragdrop, InventorySlot slot)
    {
        InventorySlot oldSlot = null;

        if (slot == null && dragdrop.isLibraryItem)
        {
            Destroy(dragdrop.gameObject);
            return;
        }

        if (!dragdrop.isLibraryItem)
        {
            for (int i = 0; i < _inventorySlots.Count; i++)
            {
                if (_inventorySlots[i].InventoryDragDropUI == dragdrop)
                {
                    oldSlot = _inventorySlots[i];
                    break;
                }
            }
        }

        if (!dragdrop.isLibraryItem) // if in toolbar
        {
            // swap 
            DragDropUI dragDropToSwapWith = slot.InventoryDragDropUI;

            if (dragDropToSwapWith != null && oldSlot != null)
            {
                oldSlot.Bind(dragDropToSwapWith);
            }
            slot.Bind(dragdrop);

        }

        if (dragdrop.isLibraryItem) // if from library
        {
            slot.Clear();
            GameObject copy = Instantiate(dragdrop.gameObject, slot.transform.position, Quaternion.identity, slot.transform);
            
            slot.Bind(copy.GetComponent<DragDropUI>());

            Destroy(dragdrop.gameObject);
            
            // i dont understand why unity disables instantiated components on objects that arent prefabs
            copy.GetComponent<RawImage>().enabled = true;
            copy.GetComponent<PointerSelect>().enabled = true;
            copy.GetComponent<SampleObject>().enabled = true;
            copy.GetComponent<DragDropUI>().enabled = true;

        }


        UpdateToolbarConfiguration();

    }

    void UpdateToolbarConfiguration()
    {
        List<string> collection = new List<string>();

        for (int i = 0; i < _inventorySlots.Count; i++)
        {
            if (_inventorySlots[i].InventoryDragDropUI.transform.TryGetComponent<SampleObject>(out var so))
            {
                collection.Insert(i, so.SampleData.ID);
            }
            else
            {
                collection.Insert(i, null);
            }
        }

        _toolbarSampleCollection.IDC.Clear();

        _toolbarSampleCollection.IDC = collection;

        DataStorage.Instance.ProjectData.ToolbarConfiguration.IDC = _toolbarSampleCollection.IDC;
    }

    void OnProjectDataLoaded(ProjectData projectData)
    {
        // clear existing items
        if (_toolbarSampleCollection.IDC.Count != 0)
        {
            _toolbarSampleCollection.IDC.Clear();
        }

        // get project toolbar data
        var copy = new List<string>(projectData.ToolbarConfiguration.IDC);
       
        AssignItems(copy);

        SetToolbarSelection(null);
    }


    public async void AssignItems(List<string> library)
    {
        if (library.Count != 0)
        {
            for (int i = 0; i < library.Count; i++)
            {
                if (i >= _inventorySlots.Count) return;

                if (library[i] == "-1") { continue; }
                
                var item = await AssetBuilder.Instance.GetToolbarItem(library[i]);

                if (item.transform.TryGetComponent<DragDropUI>(out var dragdrop))
                {
                    _inventorySlots[i].Bind(dragdrop);
                }

                //item.transform.localScale = Vector3.one;
            }

            _toolbarSampleCollection.IDC = library;
        }
    }

    public void OnNewGameMode(GameState state)
    {
        if(state == GameState.Library)
        {
            var layout = transform.GetChild(0).GetComponent<GridLayoutGroup>();
            layout.constraintCount = 10;
            layout.childAlignment = TextAnchor.LowerRight;
            layout.padding.right = 40;
            foreach (var item in _toolbarButtons)
            {
                item.SetActive(false);
            }
        }

        if(GameState.Gameplay == state)
        {
            var layout = transform.GetChild(0).GetComponent<GridLayoutGroup>();
            layout.constraintCount = 6;
            layout.padding.right = 0;
            layout.childAlignment = TextAnchor.MiddleCenter;
            foreach (var item in _toolbarButtons)
            {
                item.SetActive(true);
            }
        }
        
    }

    void SetToolbarSelection(string guid)
    {
        for(int i = 0; i < _inventorySlots.Count; i++)
        {
            if (_inventorySlots[i].InventoryDragDropUI == null) continue;

            if (_inventorySlots[i].InventoryDragDropUI.TryGetComponent<RawImage>(out var img))
            {
                if(_inventorySlots[i].InventoryDragDropUI.TryGetComponent<SampleObject>(out var so))
                {
                    if(so.SampleData.ID == guid)
                    {
                        img.CrossFadeAlpha(1f, .1f, false);
                    }
                    else
                    {
                        img.CrossFadeAlpha(.5f, .1f, false);
                    }
                }   
                continue;
            }

        }   
    }
    private void OnDisable()
    {
        DataStorage.ProjectDataSet -= OnProjectDataLoaded;
        FileManager.NewSampleSelected -= SetToolbarSelection;
        Events.DragDropFoundNewContainer -= OnDragDropFoundNewContainer;
        GameManager.StateChanged -= OnNewGameMode;
    }
}
