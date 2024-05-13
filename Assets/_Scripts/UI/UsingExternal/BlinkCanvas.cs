using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlinkCanvas : MonoBehaviour
{
    [SerializeField] CanvasGroup _cg;

    [SerializeField] float _time;

    private void Awake()
    {
        _cg = GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        LeanTween.value(gameObject, .5f, .8f, _time).setOnUpdate(setAlpha).setLoopPingPong();
    }

    private void OnDisable()
    {
        LeanTween.cancel(gameObject);
    }

    void setAlpha(float val)
    {
        _cg.alpha = val;
    }
}
