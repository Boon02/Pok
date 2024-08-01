using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class EnssentialObjectsSpawner : MonoBehaviour
{
    [SerializeField] private GameObject enssentialObjects;

    private void Awake()
    {
        var exitingObjects = FindObjectsOfType<EnssentialObjects>();
        if (exitingObjects.Length == 0)
        {
            Instantiate(enssentialObjects, transform.position, quaternion.identity);
        }
    }
}

