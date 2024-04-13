using UnityEngine.UI;
using UnityEngine;

public class ContextColliders : MonoBehaviour
{
    [SerializeField] private Transform _dragger, _remove, _copy, _scareCrow;
    [SerializeField] private RectTransform _draggerUI, _removeUI, _copyUI, _scareCrowUI;
    private Camera _cam;
    [SerializeField] private CanvasGroup _cg;
    private void Start()
    {
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
        } else
        {
            _cg.alpha = 1;
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
        var dWorldPosition = sequencer.transform.position + Vector3.back * Config.CellSize * (sequencer.RowAmount - 1) / 2f + new Vector3(Config.CellSize * (sequencer.StepAmount - 1) * .5f, 0, 0);

        
        var dScreenPosition = _cam.WorldToScreenPoint(dWorldPosition);//+ Vector3.forward * Config.CellSize

        var upperCenter = sequencer.transform.position + Vector3.forward * Config.CellSize * 2f + new Vector3(Config.CellSize * (sequencer.StepAmount - 1) * .5f, 0, 0);
        var scScreenPosition = _cam.WorldToScreenPoint(upperCenter);

        _scareCrow.transform.position = upperCenter;
        _scareCrowUI.position = scScreenPosition;

        _dragger.transform.position = dWorldPosition;
        _draggerUI.position = dScreenPosition;

        var offset = new Vector3(Config.CellSize * 2, 0, 0);
        var rWorldPosition = upperCenter + offset;
        var rScreenPosition = _cam.WorldToScreenPoint(rWorldPosition);

        _remove.transform.position = rWorldPosition;
        _removeUI.position = rScreenPosition;

        var cWorldPosition = upperCenter - offset;
        var cScreenPosition = _cam.WorldToScreenPoint(cWorldPosition);

        _copy.transform.position = cWorldPosition;
        _copyUI.position = cScreenPosition;
    }
}
