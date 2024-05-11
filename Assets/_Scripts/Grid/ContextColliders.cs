using UnityEngine.UI;
using UnityEngine;

public class ContextColliders : MonoBehaviour
{
    [SerializeField] private Transform _dragger, _remove, _copy, _scareCrow;
    [SerializeField] private RectTransform _draggerUI, _removeUI, _copyUI, _scareCrowUI;
    private Camera _cam;
    [SerializeField] private CanvasGroup _cg, _tooltip;
    private void Start()
    {
        //_cam = GameObject.FindWithTag("OverlayCamera").GetComponent<Camera>();
        //_cam = Prefabs.Instance.CanvasCamera;
        _cam = Camera.main;
        //SetContextMenu(false);
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
        if(!b)
        {
            _cg.alpha = 0;
            _tooltip.ToggleCanvasGroup(false);
        } else
        {
            _cg.alpha = 1;
            _tooltip.ToggleCanvasGroup(true);
        }
        _dragger.gameObject.SetActive(b);
        _remove.gameObject.SetActive(b);
        _copy.gameObject.SetActive(b);
        _scareCrow.gameObject.SetActive(b);
    }

    private void MoveWithSequencer(Sequencer s, Vector2 d)
    {
        //transform.position += new Vector3(d.x, 0f, d.y) * Config.CellSize;

        PositionHitboxes(s);
    }
    public void PositionHitboxes(Sequencer sequencer)
    {
        //var horizontalOffset = new Vector3(Config.CellSize * 2, 0, 0);
        //var offset = (sequencer.StepAmount - 1) * .5f * Config.CellSize;
        //if(offset < Config.CellSize * 2) offset = Config.CellSize * 2;
        //var offset = new Vector3(Config.CellSize * 2, 0, 0);
        var horizontalOffset = new Vector3(Config.CellSize * 4, 0, 0);
        //var upperCenter = sequencer.transform.position + Vector3.forward * Config.CellSize * 2f + new Vector3(Config.CellSize * (sequencer.StepAmount - 1) * .5f, 0, 0);
        var middleCenter = sequencer.transform.position + Vector3.back * Config.CellSize * ((sequencer.RowAmount - 1) * .5f) + new Vector3(Config.CellSize * (sequencer.StepAmount - 1) * .5f, 0, 0);
        //var lowerCenter = sequencer.transform.position + Vector3.back * Config.CellSize * (sequencer.RowAmount) + new Vector3(Config.CellSize * (sequencer.StepAmount - 1) * .5f, 0, 0);

        _dragger.transform.position = middleCenter;
        _draggerUI.position = /*_cam.WorldToScreenPoint(middleCenter);*/ middleCenter;

        _scareCrow.transform.position = middleCenter + (Vector3.forward * (Config.CellSize * 4));

        //_scareCrow.transform.position = upperCenter;
        //_scareCrowUI.position = _cam.WorldToScreenPoint(upperCenter); 
        _scareCrowUI.position = middleCenter + (Vector3.forward * (Config.CellSize * 4));

        var middleRight = middleCenter + horizontalOffset;

        _remove.transform.position = middleRight;
        //_removeUI.position = _cam.WorldToScreenPoint(middleRight);
        _removeUI.position = middleRight;

        var middleLeft = middleCenter - horizontalOffset;

        _copy.transform.position = middleLeft;
        //_copyUI.position = _cam.WorldToScreenPoint(middleLeft);
        _copyUI.position = middleLeft;

    }
}
