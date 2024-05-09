using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.CullingGroup;

public class PreviewDisplayer : MonoBehaviour
{
    [SerializeField] private Transform _scalerParent;

    [SerializeField] private TMPro.TextMeshProUGUI _nameText;

    private void OnEnable()
    {
        Events.SetSelectedGuid += OnSetSelectedGuid;
        GameManager.StateChanged += OnStateChanged;
    }

    private void OnStateChanged(GameState state)
    {
        if(state == GameState.Library)
        {
            OnSetSelectedGuid(AssetBuilder.Instance.SelectedGuid);
        }
       
    }

    private async void OnSetSelectedGuid(string guid)
    {
        _scalerParent.DestroyChildren();

        if (string.IsNullOrEmpty(AssetBuilder.Instance.SelectedGuid)) return;
        var so = await AssetBuilder.Instance.GetSampleObject(guid);
        
        so.transform.SetParent(_scalerParent);

        so.transform.localScale = Vector3.one;
        //so.transform.localPosition = Vector3.zero;
        so.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(18f, 0, 0));
        so.transform.position = _scalerParent.position;


        so.transform.gameObject.layer = LayerMask.NameToLayer("UI");
        foreach (Transform child in so.transform)
        {
            child.gameObject.layer = LayerMask.NameToLayer("UI");
            foreach (Transform child2 in child)
            {
                child2.gameObject.layer = LayerMask.NameToLayer("UI");
            }
        }

        _nameText.text = so.SampleData.Name;

        Events.OnScaleBounce?.Invoke(so.gameObject);
        //so.gameObject.transform.SetParent(_scalerParent);
        //if (AssetBuilder.Instance.SelectedGuid == guid)
        //{
        //    SampleObject so = AssetBuilder.Instance.SelectedSampleObject;
        //    if (so != null)
        //    {
        //        if (so.Preview != null)
        //        {
        //            so.Preview.transform.SetParent(_scalerParent);
        //            so.Preview.transform.localPosition = Vector3.zero;
        //            so.Preview.transform.localRotation = Quaternion.identity;
        //            so.Preview.transform.localScale = Vector3.one;
        //        }
        //    }
        //}
    }   

    private void OnDisable()
    {
        Events.SetSelectedGuid -= OnSetSelectedGuid;
        GameManager.StateChanged += OnStateChanged;

    }
}
