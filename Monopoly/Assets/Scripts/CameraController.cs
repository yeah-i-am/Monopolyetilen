using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class CameraController : MonoBehaviour
{
    private Vector3 velocity = Vector3.zero;
    private Vector3 zoomVelocity = Vector3.zero;
    private Vector3 lookAtPoint;
    private float oldMousePosX = 0;
    private float rotation = 0;
    private float eah = 1 / 50;

    void Start()
    {
        float c = zoomDespeed;

        for (int i = 0; i < 50; i++)
        {
            eah += c / 50;
            c *= zoomDespeed;
        }
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
        if (Input.GetKey(KeyCode.Q))
            rotation += rotationKeysSpeed;

        if (Input.GetKey(KeyCode.E))
            rotation -= rotationKeysSpeed;

        if (Input.GetKey(KeyCode.Mouse1))
            rotation += (Input.mousePosition.x - oldMousePosX) * rotationMouseSpeed;

        oldMousePosX = Input.mousePosition.x;

        return rotation;
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
            velocity *= movementSpeed / math.sqrt(sqrLength);

        velocity *= movementDeaccseleration;
        zoomVelocity *= zoomDespeed;

        RaycastHit hit;

        bool deskHit = Physics.Raycast(transform.position, transform.forward, out hit);

        /* надо ещё запретить выходить за границы доски
         * но это в другой раз */

        lookAtPoint = hit.point;
    }

    private void Update()
    {
        transform.Translate(Quaternion.Euler(-transform.rotation.eulerAngles.x, 0, 0) * velocity * Time.deltaTime);

        if (smoothRotation)
        {
            transform.RotateAround(lookAtPoint, Vector3.up, GetInputRotation() * Time.deltaTime);
            rotation *= (1f - Time.deltaTime);
        }
        else
        {
            transform.RotateAround(lookAtPoint, Vector3.up, GetInputRotation());
            rotation = 0;
        }

        /* Zoom zoom */
        zoomVelocity += GetInputCameraTranslationDirection() * zoomSpeed;
        
        var newDist = Vector3.Distance(lookAtPoint, transform.position) - zoomVelocity.z * eah;

        if (newDist < minZoomDist || newDist > maxZoomDist)
            zoomVelocity -= GetInputCameraTranslationDirection() * zoomSpeed;

        transform.Translate(zoomVelocity * Time.deltaTime);
    }

    [Header("Movement Settings")]
    [Tooltip("speeed")]
    public float movementSpeed = 30f;

    [Tooltip("accsel")]
    public float movementAccseleration = 60f;

    [Tooltip("inverse accsel"), Range(0.001f, 0.999f)]
    public float movementDeaccseleration = 0.9f;

    [Header("Zoom Settings")]
    [Tooltip("zoom zoom")]
    public float zoomSpeed = 3f;

    [Tooltip("no zoom zoom"), Range(0.001f, 0.999f)]
    public float zoomDespeed = 0.9f;

    [Tooltip("max zoom distance")]
    public float maxZoomDist = 30f;

    [Tooltip("min zoom distance")]
    public float minZoomDist = 3f;

    [Header("Rotation Settings")]
    [Tooltip("speed but for rotation for mouse")]
    public float rotationMouseSpeed = 30f;

    [Tooltip("speed but for rotation for keys")]
    public float rotationKeysSpeed = 1f;

    [Tooltip("shit")]
    public bool smoothRotation = false;

}
