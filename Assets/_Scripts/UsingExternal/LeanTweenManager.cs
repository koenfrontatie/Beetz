using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeanTweenManager : MonoBehaviour
{
    //public List<LTDescr> Tweens = new List<LTDescr>();
    //public List<int> TweenId = new List<int>();
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
        Debug.Log($"leantween animate {obj}");
        var startScale = obj.transform.localScale;
        var tween = LeanTween.scale(obj, startScale * 1.2f, .15f).setEaseInBounce().setLoopPingPong(1);
        //Tweens.Add(tween);
        //TweenId.Add(tween.id);
    }
}
