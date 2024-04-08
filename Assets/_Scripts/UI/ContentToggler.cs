using UnityEngine;
using UnityEngine.UI;

public class ContentToggler : MonoBehaviour
{
    [SerializeField] Sprite sprite1;
    [SerializeField] Sprite sprite2;

    Image targetImage;
    bool oneActive;

    private void Awake()
    {
        oneActive = true;
        targetImage = GetComponent<Image>(); 
    }

    public void ToggleContent()
    {
        if(oneActive)
        {
            targetImage.sprite = sprite2;
            oneActive = false;
        } else
        {
            targetImage.sprite = sprite1;
            oneActive = true;
        }
    }

    public void ToggleContentBool(bool b)
    {
        if (b)
        {
            targetImage.sprite = sprite2;
            oneActive = false;
        }
        else
        {
            targetImage.sprite = sprite1;
            oneActive = true;
        }
    }
}
