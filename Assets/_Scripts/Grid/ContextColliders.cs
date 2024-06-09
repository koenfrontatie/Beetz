using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using JetBrains.Annotations;

public class ContextColliders : MonoBehaviour
{
    [SerializeField] private Transform _dragger, _remove, _copy, _scareCrow, _UL, _UC, _UR, _CR, _LR, _LC, _LL, _CL;

    private List<Transform> _resizers;

    [SerializeField] private RectTransform _draggerUI, _removeUI, _copyUI, _scareCrowUI, _ULUI, _UCUI, _URUI, _CRUI, _LRUI, _LCUI, _LLUI, _CLUI;
    private Camera _cam;
    [SerializeField] private CanvasGroup _cg;
    private void Start()
    {
        //_cam = GameObject.FindWithTag("OverlayCamera").GetComponent<Camera>();
        //_cam = Prefabs.Instance.CanvasCamera;
        _cam = Camera.main;
        //SetContextMenu(false);
        _resizers = new List<Transform> { _UL, _UC, _UR, _CR, _LR, _LC, _LL, _CL };
    }

    private void OnEnable()
    {
        Events.MoveSequencer += MoveWithSequencer;
    }

    private void OnDisable()
    {
        Events.MoveSequencer -= MoveWithSequencer;
    }

    public void SetContextMenu(bool b)
    {
        var sequencerCanvas = SequencerManager.Instance.LastInteracted.transform.GetComponentInChildren<SequencerCanvas>();

        if (!b)
        {
            _cg.alpha = 0;
            sequencerCanvas.ToggleSelect(false);
            //var sequencerCanvas = SequencerManager.Instance.LastInteracted.transform.GetComponentInChildren<SequencerCanvas>();
        } else
        {
            _cg.alpha = 1;
            sequencerCanvas.ToggleSelect(true);
            SetUpperIcons(true);


            //_sequencerCanvas.alpha = 1;
        }
        _dragger.gameObject.SetActive(b);
        _remove.gameObject.SetActive(b);
        _copy.gameObject.SetActive(b);
        _scareCrow.gameObject.SetActive(b);
    }

    public void SetUpperIcons(bool b)
    {
        _remove.gameObject.SetActive(b);
        _removeUI.gameObject.SetActive(b);
        _copy.gameObject.SetActive(b);
        _copyUI.gameObject.SetActive(b);
        _scareCrow.gameObject.SetActive(b);
        _scareCrowUI.gameObject.SetActive(b);   
    }

    private void MoveWithSequencer(Sequencer s, Vector2 d)
    {
        //transform.position += new Vector3(d.x, 0f, d.y) * Config.CellSize;

        PositionHitboxes(s);
    }

    public void PositionHitboxes(Vector3 startPosition, Vector2 dimensions)
    {
        //var horizontalOffset = new Vector3(Config.CellSize * 2, 0, 0);
        var offset = (dimensions.x - 1) * .5f * Config.CellSize;
        if (offset < Config.CellSize * 2) offset = Config.CellSize * 2;

        var xPixel = new Vector3(Config.CellSize, 0, 0);
        var horizontalOffset = new Vector3(offset, 0, 0);
        var highestUp = startPosition + Vector3.forward * Config.CellSize * 3f + new Vector3(Config.CellSize * (dimensions.x - 1) * .5f, 0, 0);

        var upperCenter = startPosition + Vector3.forward * Config.CellSize + new Vector3(Config.CellSize * (dimensions.x - 1) * .5f, 0, 0);

        var lowerCenter = startPosition + Vector3.back * (Config.CellSize * dimensions.y) + new Vector3(Config.CellSize * (dimensions.x - 1) * .5f, 0, 0);

        var middleCenter = startPosition + Vector3.back * Config.CellSize * ((dimensions.y - 1) * .5f) + new Vector3(Config.CellSize * (dimensions.x - 1) * .5f, 0, 0);
        var middleRight = middleCenter + horizontalOffset;
        var middleLeft = middleCenter - horizontalOffset;
        var upperRight = upperCenter + horizontalOffset;
        var upperLeft = upperCenter - horizontalOffset;

        //var lowerCenter = sequencer.transform.position + Vector3.back * Config.CellSize * (dimensions.y) + new Vector3(Config.CellSize * (sequencer.StepAmount - 1) * .5f, 0, 0);

        _dragger.transform.position = middleCenter;
        _draggerUI.position = /*_cam.WorldToScreenPoint(middleCenter);*/ middleCenter;

        _scareCrow.transform.position = highestUp;
        //_scareCrowUI.position = _cam.WorldToScreenPoint(upperCenter); 
        _scareCrowUI.position = highestUp;



        _remove.transform.position = highestUp + horizontalOffset;
        //_removeUI.position = _cam.WorldToScreenPoint(upperRight);
        _removeUI.position = highestUp + horizontalOffset;


        _copy.transform.position = highestUp - horizontalOffset;
        //_copyUI.position = _cam.WorldToScreenPoint(upperLeft);
        _copyUI.position = highestUp - horizontalOffset;

        _UL.transform.position = _ULUI.position = upperCenter - horizontalOffset - xPixel;
        _UC.transform.position = _UCUI.position = upperCenter;
        _UR.transform.position = _URUI.position = upperRight + xPixel; ;
        _CR.transform.position = _CRUI.position = middleRight + xPixel;
        _LR.transform.position = _LRUI.position = lowerCenter + horizontalOffset + xPixel;
        _LC.transform.position = _LCUI.position = lowerCenter;
        _LL.transform.position = _LLUI.position = lowerCenter - horizontalOffset - xPixel;
        _CL.transform.position = _CLUI.position = middleLeft - xPixel;

    }

    public void PositionHitboxes(Sequencer sequencer)
    {
        //var horizontalOffset = new Vector3(Config.CellSize * 2, 0, 0);
        var offset = (sequencer.StepAmount - 1) * .5f * Config.CellSize;
        if(offset < Config.CellSize * 2) offset = Config.CellSize * 2;

        var xPixel = new Vector3(Config.CellSize, 0, 0);
        var horizontalOffset = new Vector3(offset, 0, 0);
        var highestUp = sequencer.transform.position + Vector3.forward * Config.CellSize * 3f + new Vector3(Config.CellSize * (sequencer.StepAmount - 1) * .5f, 0, 0);
        
        var upperCenter = sequencer.transform.position + Vector3.forward * Config.CellSize + new Vector3(Config.CellSize * (sequencer.StepAmount - 1) * .5f, 0, 0);

        var lowerCenter = sequencer.transform.position + Vector3.back * (Config.CellSize * sequencer.RowAmount) + new Vector3(Config.CellSize * (sequencer.StepAmount - 1) * .5f, 0, 0);
        
        var middleCenter = sequencer.transform.position + Vector3.back * Config.CellSize * ((sequencer.RowAmount - 1) * .5f) + new Vector3(Config.CellSize * (sequencer.StepAmount - 1) * .5f, 0, 0);
        var middleRight = middleCenter + horizontalOffset;
        var middleLeft = middleCenter - horizontalOffset;
        var upperRight = upperCenter + horizontalOffset;
        var upperLeft = upperCenter - horizontalOffset;

        //var lowerCenter = sequencer.transform.position + Vector3.back * Config.CellSize * (sequencer.RowAmount) + new Vector3(Config.CellSize * (sequencer.StepAmount - 1) * .5f, 0, 0);

        _dragger.transform.position = middleCenter;
        _draggerUI.position = /*_cam.WorldToScreenPoint(middleCenter);*/ middleCenter;

        _scareCrow.transform.position = highestUp;
        //_scareCrowUI.position = _cam.WorldToScreenPoint(upperCenter); 
        _scareCrowUI.position = highestUp; 



        _remove.transform.position = highestUp + horizontalOffset;
        //_removeUI.position = _cam.WorldToScreenPoint(upperRight);
        _removeUI.position = highestUp + horizontalOffset;


        _copy.transform.position = highestUp - horizontalOffset; 
        //_copyUI.position = _cam.WorldToScreenPoint(upperLeft);
        _copyUI.position = highestUp - horizontalOffset; 

        _UL.transform.position = _ULUI.position = upperCenter - horizontalOffset - xPixel;
        _UC.transform.position = _UCUI.position = upperCenter;
        _UR.transform.position = _URUI.position = upperRight + xPixel; ;
        _CR.transform.position = _CRUI.position = middleRight + xPixel; 
        _LR.transform.position = _LRUI.position = lowerCenter + horizontalOffset + xPixel;
        _LC.transform.position = _LCUI.position = lowerCenter;
        _LL.transform.position = _LLUI.position = lowerCenter - horizontalOffset - xPixel;
        _CL.transform.position = _CLUI.position = middleLeft - xPixel;

    }

    public int GetClosestCollider(Vector3 pos)
    {
        float closestDistance = 1000f;
        int index = -1;

        for(int i = 0; i < 8; i++)
        {
            var d = Vector3.Distance(pos, _resizers[i].position);

            if (d < closestDistance)
            {
                index = i;
                closestDistance = d;
            }
        }

        return index;
    }
}

//foreach (var (collider, position) in colliderPositions)
//{
//    float distance = Vector2.Distance(currentCell, position);
//    if (distance < closestDistance)
//    {
//        closestDistance = distance;
//        closestCollider = collider;
//    }
//}
