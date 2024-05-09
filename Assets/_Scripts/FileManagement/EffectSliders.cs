//using Deform;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine.UI;
//using UnityEngine;

//public class EffectSliders : MonoBehaviour
//{
//    //private List<IDeformable> _deformers = new List<IDeformable>();
//    [Header("Distortion Deformer")]
//    [SerializeField] private Slider _distortionSlider;
//    //[SerializeField] private float _dDeformFactor;
//    //[SerializeField] private SimplexNoiseDeformer _noiseDeformer;

//    [Header("Reverb Deformer")]
//    [SerializeField] private Slider _reverbSlider;
//    //[SerializeField] private float _rDeformFactor;
//    //[SerializeField] private SquashAndStretchDeformer _squashAndStretchDeformer;
//    //private List<Slider> _sliders = new List<Slider>();

//    [SerializeField] private EffectBaker _baker;

//    //[SerializeField] public LabDisplayer LabDisplayer;

//    private void Start()
//    {
//        //for(int i = 0; i < _sliders.Count; i++)
//        //{
//        //    _sliders[i].onValueChanged.AddListener(OnSliderChanged);
//        //}
//        _distortionSlider.onValueChanged.AddListener(OnDistortionChanged);
//        _reverbSlider.onValueChanged.AddListener(OnReverbChanged);

//        OnDistortionChanged(0);
//        OnReverbChanged(0);
//    }

//    void OnDistortionChanged(float val)
//    {
//        _baker.AddReverbToSelectedSampleObject(val, AssetBuilder.Instance.sele)
//        //_noiseDeformer.Factor = val * _dDeformFactor;


//    }

//    void OnReverbChanged(float val)
//    {
//        _squashAndStretchDeformer.Factor = val * _rDeformFactor;
//    }
//}
