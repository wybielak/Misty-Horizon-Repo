using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraZoomScript : MonoBehaviour // Klasa do prostego oddalania kamery, jak na razie przypisana do slidera
{
    [SerializeField]
    Camera _camera;

    [SerializeField]
    Slider _zoomSlider;

    float _sliderVal;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update() // Oddalamy kamerê o wartoœæ slidera i to wsm tyle
    {
        _sliderVal = _zoomSlider.value;

        if (_sliderVal > 0 )
            _camera.transform.localPosition = new Vector3(0, 57.3f + (_sliderVal * 2), -33.65f - _sliderVal);
    }
}
