using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WindowMoverUI : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private Canvas _canvas;

    [SerializeField]
    float _distance;
    
    bool up = false;
    //private void OnEnable()
    //{
    //    GameManager.StateChanged += OnStateChanged;
    //}

    ////public void OnStateChanged(GameState state)
    ////{
    ////    switch (state)
    ////    {

    ////        case GameState.Library:

    ////            ToggleAnimation();

    ////            break;
    ////    }
    ////}

    //private void OnDisable()
    //{
    //    GameManager.StateChanged += OnStateChanged;
    //}

    public async void ToggleAnimation()
    {
        if(!up)
        {
            //Events.AnimateRectToTarget?.Invoke(_rectTransform, _distance);
            //await Utils.LerpToTarget(_rectTransform.gameObject, _rectTransform.gameObject.transform.position + new Vector3(0, _distance * _canvas.scaleFactor, 0), .2f);
                            // i did times * .1f because plane dist is 10f...
            await Utils.LerpRectToTarget(_rectTransform, _rectTransform.gameObject.transform.position + new Vector3(0, _distance * _canvas.scaleFactor * .01f, 0), .2f);
            up = true;

        } else
        {
            await Utils.LerpRectToTarget(_rectTransform, _rectTransform.gameObject.transform.position + new Vector3(0, -_distance * _canvas.scaleFactor * .01f, 0), .2f);
            //await Utils.LerpToTarget(_rectTransform.gameObject, _rectTransform.gameObject.transform.position + new Vector3(0, -_distance * _canvas.scaleFactor, 0), .2f);
            up = false;
        }
    }
}
