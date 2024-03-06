using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeanTweenManager : MonoBehaviour
{
    public List<LTDescr> Tweens = new List<LTDescr>();
    private void OnEnable()
    {
        Events.OnScaleBounce += ScaleBounce;
    }

    private void OnDisable()
    {
        Events.OnScaleBounce -= ScaleBounce;
    }

    void Start()
    {
        LeanTween.reset();
    }

    void ScaleBounce(GameObject obj)
    {
        var startScale = obj.transform.localScale;
        //var tween = LeanTween.scale(obj, startScale * 1.2f, .1f).setEaseInOutBounce().setOnComplete(() => LeanTween.scale(obj, startScale, .2f));
        var tween = LeanTween.scale(obj, startScale * 1.2f, .1f).setEaseInOutBounce().setLoopPingPong(1);
        Tweens.Add(tween);
    }
}
