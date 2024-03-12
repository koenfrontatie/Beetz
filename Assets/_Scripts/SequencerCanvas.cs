using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequencerCanvas : MonoBehaviour
{
    [SerializeField] Sequencer _seq;
    [SerializeField] Canvas _canvas;
    void Start()
    {
        _canvas.worldCamera = Camera.main;
        _canvas.transform.localPosition += new Vector3(Config.CellSize * (_seq.StepAmount-1) * .5f, 0 ,0);
            
            //_seq.StepAmount
    }

    
}
 