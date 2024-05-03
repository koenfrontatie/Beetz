using UnityEngine;
using UnityEngine.EventSystems;

namespace FileManagement
{
    public class FileButton : MonoBehaviour, IPointerClickHandler
    {
        public string Path;
        public TMPro.TextMeshProUGUI Text;

        public void AssignPathToButton(string path)
        {
            Text.text = System.IO.Path.GetFileName(path);
            Path = path;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            //Debug.Log($"FileButton.OnClick: {Path}");
        }

    }
}
