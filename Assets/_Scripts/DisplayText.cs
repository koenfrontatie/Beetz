using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayText : MonoBehaviour
{
    private TMP_Text text;
    void Start()
    {
        text = GetComponent<TMP_Text>();
        text.color = Config.DisplayColor;
    }
}
