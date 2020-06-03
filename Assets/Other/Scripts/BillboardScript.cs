using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Script that handles billboard rendering
public class BillboardScript : MonoBehaviour
{
    private Camera _camera;//The current camera (That is rendering the scene)
    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main;//Try to get a valid camera at start
    }

    // Update is called once per frame
    void Update()
    {
        //Make the billboard have the same forward direction as the camera
        //TODO: Turn this into a main billboard manager to save on performance and optimize the camera handling
        if (_camera != null && _camera.gameObject.activeSelf) { transform.forward = _camera.transform.forward; }
        else { _camera = Camera.main; }//Try to get a valid camera as soon as possible
    }
    private void LateUpdate()
    {
        //Make the billboard have the same forward direction as the camera
        if (_camera != null) { transform.forward = _camera.transform.forward; }
    }
}
