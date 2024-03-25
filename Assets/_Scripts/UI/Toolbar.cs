using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toolbar : MonoBehaviour
{
    [SerializeField] Transform _TBC;
    [SerializeField] Transform _TBI;

    public InventorySlot[] _containers;

    private GameObject[] _toolbarConfiguration = new GameObject[Config.ToolbarCount];

    private void Awake()
    {

    }
    private void OnDisable()
    {
        //Events.OnLibraryLoaded -= () => AssignItems(SampleManager.Instance.Library);
        //Events.OnInventoryChange -= UpdateConfiguration;

        Events.OnToolbarLoaded -= () => AssignItems(ProjectData.Instance.ProjectInfo.ToolbarConfig);
    }
    void OnEnable()
    {
        Debug.Log("_TBC child count: " + _TBC.childCount); // Debug log to check the number of children

        for (int i = 0; i < Config.ToolbarCount; i++)
        {
            _containers[i] = _TBC.GetChild(i).GetComponent<InventorySlot>();
        }
        Events.OnToolbarLoaded += () => AssignItems(ProjectData.Instance.ProjectInfo.ToolbarConfig);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I)) AssignItems(ProjectData.Instance.ProjectInfo.ToolbarConfig);
    }

    public void AssignItems(List<string> library)
    {
        //for(int i = 0; i < library.Count; i++)
        //{
        //    Debug.Log(library[i]);
        //}

        for(int i = 0; i < Config.ToolbarCount; i++)
        {
            Debug.Log(library[i]);

            var item = ObjectMaker.Instance.ToolbarItem(library[i]);

            item.transform.position = _containers[i].transform.position;

            item.transform.parent = _TBI;

            _toolbarConfiguration[i] = item;
        }

        //ToolbarConfiguration.Capacity = _containers.Count;

        Events.OnInventoryChange?.Invoke();
    }

    //public void UpdateConfiguration()
    //{
    //    for(int i = 0; i < _containers.Count; i++)
    //    {
    //        for(int j = 0; j < ToolbarConfiguration.Count; j++)
    //        {
    //            if (ToolbarConfiguration[j].transform.position == _containers[i].transform.position) {

    //                var so = ToolbarConfiguration[j].GetComponent<SampleObject>();
    //                var inv = _containers[i].GetComponent<InventorySlot>();
    //                ToolbarConfiguration[i] = so.gameObject;
    //                inv.Bind(so); // this can be improved ...
    //            }
    //        }
    //    }
    //}
}
