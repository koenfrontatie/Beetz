using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenericButton : MonoBehaviour
{
    public int DownwardPixels = 25;
    public Button Button;

    private void OnEnable()
    {
        Button.onClick.AddListener(Animate);
    }

    private void OnDisable()
    {
        Button.onClick.RemoveListener(Animate);
    }

    void Animate()
    {
        Events.AnimateButton?.Invoke(this);
    }
}
