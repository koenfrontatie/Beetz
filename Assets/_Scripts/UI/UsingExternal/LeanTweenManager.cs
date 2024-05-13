using UnityEngine;

public class LeanTweenManager : MonoBehaviour
{
    //public List<LTDescr> Tweens = new List<LTDescr>();
    //public List<int> TweenId = new List<int>();

    private void OnEnable()
    {
        Events.OnScaleBounce += ScaleBounce;
        Events.MovingCameraToScarecrow += AnimateCamToScarecrow;

        Events.AnimateButton += AnimateButton;

        Events.AnimateRectToTarget += AnimateRect;

    }

    private void OnDisable()
    {
        Events.OnScaleBounce -= ScaleBounce;
        Events.MovingCameraToScarecrow -= AnimateCamToScarecrow;
        Events.AnimateButton -= AnimateButton;
        Events.AnimateRectToTarget -= AnimateRect;
    }

    void Start()
    {
        LeanTween.reset();
    }

    void ScaleBounce(GameObject obj)
    {
        if (obj == null)
        {
            Debug.LogWarning("ScaleBounce() called with a null GameObject.");
            return;
        }
        var startScale = obj.transform.localScale;
        if (obj.LeanIsTweening()) return;
        var tween = LeanTween.scale(obj, startScale * 1.2f, .15f).setEaseInBounce().setLoopPingPong(1);
        //Tweens.Add(tween);
        //TweenId.Add(tween.id);
    }

    void AnimateButton(GenericButton button)
    {
        var obj = button.Button.gameObject;

        var rect = obj.GetComponent<RectTransform>();

        if (rect == null) return;
        
        if(obj.LeanIsTweening()) return;

        LeanTween.moveY(rect, -button.DownwardPixels, .06f).setEaseOutCubic().setLoopPingPong(1); ;
    }

    void AnimateRect(RectTransform rect, float target)
    {
        if (rect == null) return;

        if (rect.gameObject.LeanIsTweening()) return;

        LTRect ltRect = new LTRect(rect.rect);

        LeanTween.move(ltRect, new Vector2(0, target), .2f);
    }

    void AnimateCamToScarecrow()
    {
       
        LeanTween.move(Camera.main.gameObject, new Vector3(0f, Camera.main.transform.position.y, 5f), .3f).setEaseInOutSine();
    }
}
