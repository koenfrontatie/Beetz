using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toolbar : MonoBehaviour
{
    [SerializeField] Transform _TBC;
    [SerializeField] Transform _TBI;

    public InventorySlot[] _containers;

    private GameObject[] _toolbarConfiguration = new GameObject[Config.ToolbarCount];

    private void OnEnable()
    {

        Events.LoadingToolbar += (data) => AssignItems(new List<string>(data));
        for (int i = 0; i < Config.ToolbarCount; i++)
        {
            _containers[i] = _TBC.GetChild(i).GetComponent<InventorySlot>();
        }
    }
    private void OnDisable()
    {
        //Events.OnLibraryLoaded -= () => AssignItems(SampleManager.Instance.Library);
        //Events.OnInventoryChange -= UpdateConfiguration;

        Events.LoadingToolbar -= (data) => AssignItems(new List<string>(data));
    }
    //void OnEnable()
    //{
    //    for (int i = 0; i < Config.ToolbarCount; i++)
    //    {
    //        _containers[i] = _TBC.GetChild(i).GetComponent<InventorySlot>();
    //    }

    //    Events.ProjectDataLoaded += (data) => AssignItems(data.IDCollection.IDC);
    //}

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I)) AssignItems(DataStorage.Instance.ProjectData.IDCollection.IDC);
    }
    IEnumerator Assign(List<string> library)
    {
        yield return new WaitForSeconds(1);

        for(int i =0; i < library.Count; i++)
        {
            Debug.Log(library[i]);

        }
        if (library.Count != 0)
        {
            for (int i = 0; i < 5; i++)
            {
                var item = ObjectMaker.Instance.ToolbarItem(i.ToString());
                item.transform.position = _containers[i].transform.position;

                item.transform.parent = _TBI;

                _toolbarConfiguration[i] = item;
            }
        }
    }
    public void AssignItems(List<string> library)
    {
        if (library.Count != 0)
        {
            for (int i = 0; i < library.Count; i++)
            {
                if (library[i] == "-1") { continue; }
                
                var item = ObjectMaker.Instance.ToolbarItem(library[i]);
                
                item.transform.position = _containers[i].transform.position;

                item.transform.parent = _TBI;

                _toolbarConfiguration[i] = item;
            }
        }
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
