using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    private string _guid;

    public SampleObject InventorySampleObject;


    public void Bind(SampleObject so)
    {
        InventorySampleObject = so;
    }
}
