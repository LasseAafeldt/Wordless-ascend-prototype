using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private float smoothTime = .2f;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Transform player;

    private Vector3 velocity;

    private void LateUpdate()
    {
        Vector3 targetYPosition = new Vector3(transform.position.x, player.position.y, transform.position.z);
        Vector3 targetPosition = targetYPosition + offset;

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
