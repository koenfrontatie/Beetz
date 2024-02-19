using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Hotbar : MonoBehaviour
{
    public int LastClicked;
    private List<Button> buttons = new List<Button>();
    private List<TextMeshProUGUI> text = new List<TextMeshProUGUI>();

    void Awake()
    {
        //foreach(Transform child in transform)
        //{
        //    if(child.TryGetComponent<Button>(out Button b)) 
        //    {
        //        buttons.Add(b);
        //        b.onClick.AddListener(() => { LastClicked = buttons.IndexOf(b); Events.OnHotbarClicked?.Invoke(LastClicked); });
        //        text.Add(b.GetComponentInChildren<TextMeshProUGUI>());
        //    }
        //}

        for(int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).TryGetComponent<Button>(out Button b))
            {
                buttons.Add(b);
                b.onClick.AddListener(() => { LastClicked = buttons.IndexOf(b); Events.OnHotbarClicked?.Invoke(LastClicked); });
                text.Add(b.GetComponentInChildren<TextMeshProUGUI>());
            }
        }
    }

    public void SetButtonText(int i, string input)
    {
        text[i].text = input;
    }
}
