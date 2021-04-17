using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Location : MonoBehaviour
{
    public Transform positionPoint;

    void Start()
    {
        GameObject.CreatePrimitive(PrimitiveType.Cube).transform.SetParent(positionPoint, false);

    }

    void Update()
    {
        
    }
}
