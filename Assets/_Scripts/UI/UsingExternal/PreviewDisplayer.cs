using UnityEngine;
using FileManagement;
using System.Threading.Tasks;

public class PreviewDisplayer : MonoBehaviour
{
    [SerializeField] private Transform _scalerParent;
    [SerializeField] private RectTransform _previewRect;
    [SerializeField] private TMPro.TextMeshProUGUI _nameText;

    [SerializeField] SampleObject _currentObject;

    private void Start()
    {
        //_scalerStartPosition = _scalerParent.position;
        //_previewStartPosition = _previewRect.anchoredPosition;

        LTRect pRect = new LTRect(_previewRect.rect);

        var moveY = LeanTween.moveLocalY(_previewRect.gameObject, 100f, 2f).setEaseInOutSine().setLoopPingPong();

        var rotateY = LeanTween.rotateAroundLocal(_scalerParent.gameObject, Vector3.up, 360f, 24f).setRepeat(-1);
    }

    private void OnEnable()
    {
        FileManager.NewSampleSelected += OnSampleSelected;
        GameManager.StateChanged += OnStateChanged;
        //FileManager.SampleUpdated += OnSampleSelected;
    }

    private void OnStateChanged(GameState state)
    {
        if(state == GameState.Library)
        {
            //_scalerParent.gameObject.SetActive(true);
            
            //_scalerParent.position = _scalerStartPosition;
            //_previewRect.anchoredPosition = _previewStartPosition;
            OnSampleSelected(FileManager.Instance.SelectedSampleGuid);

        }
        else
        {
            
            //_scalerParent.gameObject.SetActive(false);
        }
    }

    private async void OnSampleSelected(string guid)
    {
        _scalerParent.DestroyChildren();

        if (string.IsNullOrEmpty(FileManager.Instance.SelectedSampleGuid)) return;
        
        //if (_currentObject != null && string.Equals(_currentObject.SampleData.ID, guid)) return;

        //if (_currentObject != null)
        //{
        //    Destroy(_currentObject);
        //    _scalerParent.DestroyChildren();
        //}

        SampleObject so = await AssetBuilder.Instance.GetSampleObject(guid);

        _currentObject = so;

        if(_currentObject == null) return;
        //Task.Run( () =>
        //{
        //    so = await AssetBuilder.Instance.GetSampleObject(guid);
        //});
        //Debug.Log("library on selected");

        _currentObject.transform.SetParent(_scalerParent);

        _currentObject.transform.localScale = Vector3.one;
        
        _currentObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        _currentObject.transform.gameObject.layer = LayerMask.NameToLayer("UI");
        
        //_currentObject.transform.GetChild(0).transform.localPosition = Vector3.zero;
        //_currentObject.transform.GetChild(0).transform.localRotation = Quaternion.identity;

        foreach (Transform child in _currentObject.transform)
        {
            child.gameObject.layer = LayerMask.NameToLayer("UI");
            
            foreach (Transform child2 in child)
            {
                child2.gameObject.layer = LayerMask.NameToLayer("UI");
            }
        }

        _nameText.text = _currentObject.SampleData.Name;

        Events.OnScaleBounce?.Invoke(_currentObject.gameObject);
    }   

    private void OnDisable()
    {
        FileManager.NewSampleSelected -= OnSampleSelected;
        GameManager.StateChanged -= OnStateChanged;
        //FileManager.SampleUpdated -= OnSampleSelected;

    }
}
