using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCameraControls : MonoBehaviour {

    public float MoveSpeed = 10;
    public float RotateSensitivity = 2;

    private float _xRotation;
    private float _yRotation;

    public void Start()
    {
        _xRotation = -transform.localRotation.eulerAngles.x;
        _yRotation = transform.localRotation.eulerAngles.y;
    }

    void LateUpdate () {
        //If right click is held, move camera.
        if (Input.GetMouseButton(1))
        {
            _xRotation += Input.GetAxis("Mouse Y") * RotateSensitivity;
            _yRotation += Input.GetAxis("Mouse X") * RotateSensitivity;
            _xRotation = Mathf.Clamp(_xRotation, -90, 90);
            //_yRotation = Mathf.Clamp(_yRotation, -90, 90);
            transform.localRotation = Quaternion.Euler(-_xRotation, _yRotation, 0f);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        Vector3 transformation = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
        transform.Translate(transformation * MoveSpeed * Time.deltaTime, Space.Self);
	}
}
