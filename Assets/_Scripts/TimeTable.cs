using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
public class TimeTable : MonoBehaviour
{
    float timer = 0;
    [SerializeField]float expireTime = 180f;
    [SerializeField] CanvasGroup _cg;
    [SerializeField] UIController _uic;
    bool timeTableActive = false;
    [SerializeField] VideoPlayer _vp;

    private void Start()
    {
        _cg.ToggleCanvasGroup(false);
         timeTableActive = false;
    }
    void Update()
    {
        if (timeTableActive) return;

        timer += Time.deltaTime;
        
        if (timer >= expireTime)
        {
            //Debug.Log("One second has passed");
            timer = 0;
            _uic.ToggleTimeTable();
            _cg.ToggleCanvasGroup(true);
            Metronome.ResetMetronome();
            timeTableActive = true;
            _vp.Play();
        }
    }

    public void ResetTimer()
    {
        timer = 0;
        timeTableActive = false;
        _vp.Pause();
        _cg.ToggleCanvasGroup(false);
        GameManager.Instance.UpdateState(GameState.Gameplay);
    }
}
