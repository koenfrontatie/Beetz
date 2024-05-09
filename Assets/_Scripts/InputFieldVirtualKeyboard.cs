using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class InputFieldVirtualKeyboard : MonoBehaviour
{
    TMP_InputField inputField;
    VirtualKeyboard vk = new VirtualKeyboard();

    bool isWindows = false;

    private void Start()
    {
        isWindows = (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor);
        if(isWindows)
        {
            inputField = GetComponent<TMP_InputField>();
            inputField.onSelect.AddListener(OnSelect);
            inputField.onDeselect.AddListener(OnDeselect);
        }
    }

    private void OnDeselect(string arg0)
    {
        vk.HideTouchKeyboard();

    }

    void OnSelect(string text)
    {
        vk.ShowTouchKeyboard();
    }


}
