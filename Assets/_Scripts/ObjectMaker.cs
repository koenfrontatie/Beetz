using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectMaker : MonoBehaviour
{
    // This script will become a factory for hud and sample objects

    [SerializeField] private GameObject _TBIPrefab;
    
    public static ObjectMaker Instance;

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

    // this creates hud items from id
    public GameObject ToolbarItem(string guid)
    {
        GameObject item = Instantiate(_TBIPrefab);

        var image = item.GetComponent<RawImage>();
        var so = item.GetComponent<SampleObject>();

        // if template

        if(guid.Length < 3)
        {
            if(guid == "-1")
            {
                image.texture = null;
                image.color = new Color(1,1,1,0);
                return item;
            }
            
            var template = int.Parse(guid);

            image.texture = Prefabs.Instance.BaseIcons[template];

            so.SampleData = Prefabs.Instance.BaseObjects[template].SampleData;

            return item;
        }

        // to do ---- load non templates

        return item;
    }
    
    // this sample objects from id
    public GameObject SampleObject(string guid)
    {
        //GameObject item = new GameObject();

        //var image = item.GetComponent<RawImage>();
        //var so = item.GetComponent<SampleObject>();

        // if template

        if (guid.Length < 3)
        {
            var template = int.Parse(guid);


            return Prefabs.Instance.BaseObjects[template].transform.gameObject;
        }

        // to do ---- load non templates

        return null;
    }
}
