using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BlinkAnimator : MonoBehaviour
{
    public Material GridLineMat;

    public bool BlinkActive;

    public float aMin, aMax, t;

    Color minColor, maxColor;

    private void Start()
    {
        minColor = new Color(1, 1, 1, aMin);
        maxColor = new Color(1, 1, 1, aMax);
    }

    void Update()
    {
        if (!BlinkActive) return;

        var lerpedColor = Color.Lerp(minColor, maxColor, Mathf.PingPong(Time.time, t));
        GridLineMat.SetColor("_BaseColor", lerpedColor);
        GridLineMat.SetColor("_EmissionColor", lerpedColor);
    }
}
