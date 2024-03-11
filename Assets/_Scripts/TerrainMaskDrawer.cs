//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class TerrainMaskDrawer : MonoBehaviour
//{
//    [SerializeField] private Camera _renderCam;
//    [SerializeField] private RenderTexture _RT;
//    [SerializeField] private Transform _drawPixel;
//    [SerializeField] private Transform _indicator;
//    [SerializeField] private Vector2 _sequencerDimension;

    

//    private void OnEnable()
//    {
//        Events.OnGridClicked += UpdateTexture;
//    }

//    private void UpdateTexture()
//    {
//        //_renderCam.targetTexture = _RT;

//        for (int i = 0; i < _sequencerDimension.x; i ++)
//        {
//            var pixel = Instantiate(_drawPixel, transform);
//            pixel.position = _indicator.position + new Vector3(i * Config.CellSize, 0, 0);
//        }
//        //_renderCam.targetTexture = _RT;
//        _renderCam.Render();


//        //Graphics.Blit(_renderCam.activeTexture, _RT);
//    }

//    private void OnDisable()
//    {
//        Events.OnGridClicked -= UpdateTexture;
//    }

//}
