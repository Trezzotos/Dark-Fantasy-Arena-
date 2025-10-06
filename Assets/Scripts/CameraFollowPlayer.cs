using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    [Tooltip("What should the camera follow")]
    public Transform target;

    [Space]
    [Range(1f, 10f)]
    public float smoothSpeed = 5f;
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    Vector3 desiredPosition;
    Vector3 smoothedPosition;
    // Start is called before the first frame update
    void Start()
    {
        if (!target) Debug.LogWarning("Target not specified!");
    }

    // Update is called once per frame
    void LateUpdate()
    {
        desiredPosition = target.position + offset;

        // Smooth things up
        smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        transform.position = smoothedPosition;
    }
}
