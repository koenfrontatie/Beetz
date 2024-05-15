using UnityEngine;
using FileManagement;
using System.Threading.Tasks;

public class PreviewDisplayer : MonoBehaviour
{
    [SerializeField] private Transform _scalerParent;
    [SerializeField] private RectTransform _previewRect;
    [SerializeField] private TMPro.TextMeshProUGUI _nameText;

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
    }

    private void OnStateChanged(GameState state)
    {
        if(state == GameState.Library)
        {
            _scalerParent.gameObject.SetActive(true);
            
            //_scalerParent.position = _scalerStartPosition;
            //_previewRect.anchoredPosition = _previewStartPosition;


        }
        else
        {
            
            _scalerParent.gameObject.SetActive(false);
        }
    }

    private async void OnSampleSelected(string guid)
    {
        _scalerParent.DestroyChildren();

        if (string.IsNullOrEmpty(FileManager.Instance.SelectedSampleGuid)) return;

        SampleObject so;
        so = await AssetBuilder.Instance.GetSampleObject(guid);
        //Task.Run( () =>
        //{
        //    so = await AssetBuilder.Instance.GetSampleObject(guid);
        //});
        //Debug.Log("library on selected");

        so.transform.SetParent(_scalerParent);

        so.transform.localScale = Vector3.one;
        
        so.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        so.transform.gameObject.layer = LayerMask.NameToLayer("UI");
        
        //so.transform.GetChild(0).transform.localPosition = Vector3.zero;
        //so.transform.GetChild(0).transform.localRotation = Quaternion.identity;

        foreach (Transform child in so.transform)
        {
            child.gameObject.layer = LayerMask.NameToLayer("UI");
            
            foreach (Transform child2 in child)
            {
                child2.gameObject.layer = LayerMask.NameToLayer("UI");
            }
        }

        _nameText.text = so.SampleData.Name;

        Events.OnScaleBounce?.Invoke(so.gameObject);
    }   

    private void OnDisable()
    {
        FileManager.NewSampleSelected -= OnSampleSelected;
        GameManager.StateChanged -= OnStateChanged;
    }
}
