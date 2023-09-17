using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 1;
    public float zoomMinValue, zoomMaxValue, zoomScale = 1f;
    private Camera mainCamera;
    public Transform upperBound, lowerBound, leftBound, rightBound;
    public InputAction moveAction;
    public InputAction zoomAction;
    void Start()
    {
        mainCamera = Camera.main;
        moveAction.Enable();
        zoomAction.Enable();
    }

    void Update()
    {
        Vector2 moveAmount = moveAction.ReadValue<Vector2>();
        float zoomAmount = zoomAction.ReadValue<float>();
        transform.Translate(new Vector3(moveAmount.x, moveAmount.y, 0) * moveSpeed * Time.deltaTime * (mainCamera.orthographicSize / 10));

        if(transform.position.x < leftBound.position.x) transform.position = new Vector3(leftBound.position.x, transform.position.y, transform.position.z);
        else if(transform.position.x > rightBound.position.x) transform.position = new Vector3(rightBound.position.x, transform.position.y, transform.position.z);

        if(transform.position.y < lowerBound.position.y) transform.position = new Vector3(transform.position.x, lowerBound.position.y, transform.position.z);
        else if(transform.position.y > upperBound.position.y) transform.position = new Vector3(transform.position.x, upperBound.position.y, transform.position.z);

        if((zoomAmount > 0 && mainCamera.orthographicSize < zoomMaxValue) || (zoomAmount < 0 && mainCamera.orthographicSize > zoomMinValue))
        {
            mainCamera.orthographicSize += zoomAmount * zoomScale;
        }
    }
}
