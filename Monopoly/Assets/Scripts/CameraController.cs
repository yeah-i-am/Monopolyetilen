using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Vector3 velocity = Vector3.zero;
    private Vector3 cameraVelocity = Vector3.zero;

    void Start()
    {
    }

    Vector3 GetInputTranslationDirection()
    {
        Vector3 direction = Vector3.zero;
#if ENABLE_INPUT_SYSTEM
            var moveDelta = movementAction.ReadValue<Vector2>();
            direction.x = moveDelta.x;
            direction.z = moveDelta.y;
            direction.y = verticalMovementAction.ReadValue<Vector2>().y;
#else
        if (Input.GetKey(KeyCode.W))
            direction += Vector3.forward;

        if (Input.GetKey(KeyCode.S))
            direction += Vector3.back;

        if (Input.GetKey(KeyCode.A))
            direction += Vector3.left;

        if (Input.GetKey(KeyCode.D))
            direction += Vector3.right;
#endif
        return direction.normalized;
    }

    float GetInputRotation()
    {
        float rotaion = 0;

        if (Input.GetKey(KeyCode.Q))
            rotaion += 1f;

        if (Input.GetKey(KeyCode.E))
            rotaion -= 1f;

        return rotaion;
    }

    Vector3 GetInputCameraTranslationDirection()
    {
        Vector3 direction = Vector3.zero;

        direction += Vector3.forward * Input.mouseScrollDelta.y;

        return direction;
    }

    void FixedUpdate()
    {
        velocity += GetInputTranslationDirection() * movementAccseleration;

        var sqrLength = velocity.sqrMagnitude;

        if (sqrLength > movementSpeed * movementSpeed)
            velocity *= movementSpeed / Mathf.Sqrt(sqrLength);

        velocity *= movementDeaccseleration;
        cameraVelocity *= zoomDespeed;
    }

    private void Update()
    {
        transform.Translate(velocity * Time.deltaTime);
        transform.Rotate(new Vector3(0, rotaionSpeed * GetInputRotation() * Time.deltaTime, 0));

        cameraVelocity += GetInputCameraTranslationDirection() * zoomSpeed;
        Camera.transform.Translate(cameraVelocity * Time.deltaTime);
    }

    [Header("Movement Settings")]
    [Tooltip("speeed")]
    public float movementSpeed = 30f;

    [Tooltip("accsel")]
    public float movementAccseleration = 60f;

    [Tooltip("inverse accsel"), Range(0.001f, 0.999f)]
    public float movementDeaccseleration = 0.9f;

    [Header("Zoom Settings")]
    [Tooltip("Yes, that camera")]
    public GameObject Camera;

    [Tooltip("zoom zoom")]
    public float zoomSpeed = 3f;

    [Tooltip("no zoom zoom"), Range(0.001f, 0.999f)]
    public float zoomDespeed = 0.9f;

    [Header("Rotation Settings")]
    [Tooltip("speed but for rotation")]
    public float rotaionSpeed = 30f;

}
