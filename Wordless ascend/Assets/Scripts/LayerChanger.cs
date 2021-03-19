using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerChanger : MonoBehaviour
{
    [SerializeField] private GameObject targetObject;
    [SerializeField] private SingleUnityLayer groundLayer;
    [SerializeField] private SingleUnityLayer nonCollisionLayer;
    [SerializeField] private SingleUnityLayer playerLayer;

    public static bool canChangeLayer = true;

    private void OnTriggerEnter(Collider other)
    {
        if(transform.name == "LowerHitbox")
        {
            targetObject.layer = nonCollisionLayer.LayerIndex;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (transform.name == "LowerHitbox")
        {
            targetObject.layer = groundLayer.LayerIndex;
        }
    }
}
