using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tweener : MonoBehaviour
{
    [SerializeField]
    private float _rotationDistance;
    [SerializeField]
    private float _verticalDistance;
    [SerializeField]
    private float _rotationTime;
    [SerializeField]
    private float _verticalTime;

    private float _startVert;
    private float _startRotation;

    private Vector3 _startPositionv3;
    private Vector3 _startRotationv3; 

    private void Awake()
    {
        _startVert = gameObject.transform.localPosition.y;
        _startRotation = gameObject.transform.localRotation.eulerAngles.y;

        _startPositionv3 = gameObject.transform.position;
        _startRotationv3 = gameObject.transform.rotation.eulerAngles;

    }

    private void OnEnable()
    {
        transform.position = _startPositionv3;
        transform.rotation = Quaternion.Euler(_startRotationv3);

        LeanTween.cancel(gameObject);

        transform.localPosition = new Vector3(transform.localPosition.x, _startVert, transform.localPosition.z);
        transform.localRotation = Quaternion.Euler(transform.localRotation.x, _startRotation - _rotationDistance / 2, transform.localRotation.z);

        LeanTween.moveY(gameObject, transform.localPosition.y + _verticalDistance, _verticalTime).setLoopPingPong().setEase(LeanTweenType.easeInOutSine);
        _startVert = gameObject.transform.localPosition.y;
        _startRotation = gameObject.transform.localRotation.eulerAngles.y;

        LeanTween.cancel(gameObject);

        transform.localPosition = new Vector3(transform.localPosition.x, _startVert, transform.localPosition.z);
        transform.localRotation = Quaternion.Euler(transform.localRotation.x, _startRotation, transform.localRotation.z);

        LeanTween.moveLocalY(gameObject, _startVert + _verticalDistance, _verticalTime).setLoopPingPong().setEase(LeanTweenType.easeInOutSine);
        LeanTween.rotateAroundLocal(gameObject, Vector3.up, _rotationDistance, _rotationTime).setLoopPingPong().setEase(LeanTweenType.easeInOutSine);
    }

    private void OnDisable()
    {
        LeanTween.cancel(gameObject);
        gameObject.transform.position = _startPositionv3;
        gameObject.transform.rotation = Quaternion.Euler(_startRotationv3);
    }

}
