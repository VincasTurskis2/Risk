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

        Debug.Log(mainCamera.orthographicSize);
        if(transform.position.x < leftBound.position.x + mainCamera.orthographicSize * mainCamera.aspect && transform.position.x > rightBound.position.x - mainCamera.orthographicSize * mainCamera.aspect)
        {
            transform.position = new Vector3(0, transform.position.y, transform.position.z);
        }
        else if(transform.position.x < leftBound.position.x + mainCamera.orthographicSize * mainCamera.aspect) transform.position = new Vector3(leftBound.position.x + mainCamera.orthographicSize * mainCamera.aspect, transform.position.y, transform.position.z);
        else if(transform.position.x > rightBound.position.x - mainCamera.orthographicSize * mainCamera.aspect) transform.position = new Vector3(rightBound.position.x - mainCamera.orthographicSize * mainCamera.aspect, transform.position.y, transform.position.z);

        if(transform.position.y < lowerBound.position.y + mainCamera.orthographicSize && transform.position.y > upperBound.position.y - mainCamera.orthographicSize)
        {
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        }
        else if(transform.position.y < lowerBound.position.y + mainCamera.orthographicSize) transform.position = new Vector3(transform.position.x, lowerBound.position.y + mainCamera.orthographicSize, transform.position.z);
        else if(transform.position.y > upperBound.position.y - mainCamera.orthographicSize) transform.position = new Vector3(transform.position.x, upperBound.position.y - mainCamera.orthographicSize, transform.position.z);

        if((zoomAmount > 0 && mainCamera.orthographicSize < zoomMaxValue) || (zoomAmount < 0 && mainCamera.orthographicSize > zoomMinValue))
        {
            mainCamera.orthographicSize += zoomAmount * zoomScale;
        }
    }
}
