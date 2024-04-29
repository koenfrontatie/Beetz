//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.EventSystems;

//public class WindowResizerUI : MonoBehaviour
//{
//    [SerializeField] private RectTransform _rectTransform;
//    [SerializeField] private Canvas _canvas;

//    [SerializeField]
//    float _distance;

//    bool up = false;

//    public void ScaleBottom()
//    {
//        if (!up)
//        {
//            _rectTransform.offsetMin = new Vector2(_rectTransform.offsetMin.x, _rectTransform.offsetMin.y + _distance * _canvas.scaleFactor);
//            up = true;

//        }
//        else
//        {
//            _rectTransform.offsetMin = new Vector2(_rectTransform.offsetMin.x, _rectTransform.offsetMin.y - _distance * _canvas.scaleFactor);
//            up = false;
//        }
//    }
//}

using UnityEngine;
using System.Threading.Tasks;

public class WindowResizerUI : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private Canvas _canvas;

    [SerializeField]
    float _distance;

    bool up = false;

    public async void ScaleBottom(float duration = 0.2f)
    {
        Vector2 targetOffsetMin = up ? new Vector2(_rectTransform.offsetMin.x, _rectTransform.offsetMin.y - _distance * _canvas.scaleFactor) : new Vector2(_rectTransform.offsetMin.x, _rectTransform.offsetMin.y + _distance * _canvas.scaleFactor);
        Vector2 initialOffsetMin = _rectTransform.offsetMin;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            _rectTransform.offsetMin = Vector2.Lerp(initialOffsetMin, targetOffsetMin, t);
            await Task.Yield();
            elapsedTime += Time.deltaTime;
        }

        _rectTransform.offsetMin = targetOffsetMin;
        up = !up; // Toggle the up state after lerping
    }
}
